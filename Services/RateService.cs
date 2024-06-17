
using System.Runtime.CompilerServices;
using AutoMapper;
using backend.DTOs;
using backend.Exceptions;
using backend.Helper;
using backend.Models;
using backend.Repositories.IRepositories;


namespace backend.Services
{
    public class RateService
    {
        private readonly IRateRepository _rateRepository;
        private readonly IMapper _mapper;
        private readonly Generate _generate;
        private readonly AccountService _accountService;
        private readonly OrderService _orderService;
        private readonly IRateLikeRepository _rateLikeRepository;
        private readonly IProductRepository _productRepository;

        public RateService(IRateRepository rateRepository, IMapper mapper, Generate generate, AccountService accountService, OrderService orderService, IRateLikeRepository rateLikeRepository,IProductRepository productRepository)
        {
            _rateRepository = rateRepository;
            _mapper = mapper;
            _generate = generate;
            _accountService = accountService;
            _orderService = orderService;
            _rateLikeRepository = rateLikeRepository;
            _productRepository=productRepository;
        }
        public async Task<List<RateGetDTO>> GetAllRatesAsync()
        {
            var rates = await _rateRepository.GetAllAsync();
            return _mapper.Map<List<RateGetDTO>>(rates);
        }
        public async Task<List<RateGetDTO>> GetAllRatesActiveAsync()
        {
            var rates = await _rateRepository.GetAllActiveAsync();
            return _mapper.Map<List<RateGetDTO>>(rates);
        }
        public async Task<RateGetDTO> GetRateByIdAsync(long id)
        {
            var rate = await _rateRepository.GetByIdAsync(id);
            if (rate == null)
            {
                throw new NotFoundException("Danh mục không tồn tại.");
            }
            return _mapper.Map<RateGetDTO>(rate);
        }
        public async Task<RateLikeGetDTO> GetRateLikeAsync(long rateId,string token)
        {
             var userId = _accountService.ExtractUserIdFromToken(token);
            if (userId == null)
            {
                throw new NotFoundException("Có lỗi xãy ra vui lòng đăng nhập lại");
            }

            var user = await _accountService.GetUserByIdAsync(userId);
            var rate = await _rateLikeRepository.GetByIdAsync(rateId,userId);
            if (rate == null)
            {
                 return new RateLikeGetDTO{
                Id = 0,
                IsLike= true
            };
            }
            return new RateLikeGetDTO{
                Id = rate.Id,
                IsLike=rate.IsLike
            };
        }
        public async Task<List<RateGetDTO>> GetRatesByProductIddsync(long id)
        {
            var rate = await _rateRepository.GetByProductIdAsync(id);
            return _mapper.Map<List<RateGetDTO>>(rate);
        }

        public async Task<(int likeCount, int dislikeCount)> ActionAsync(long rateId, bool isLike, string token)
        {
            var userId = _accountService.ExtractUserIdFromToken(token);
            if (userId == null)
            {
                throw new NotFoundException("Có lỗi xãy ra vui lòng đăng nhập lại");
            }

            var user = await _accountService.GetUserByIdAsync(userId);
            var rate = await _rateRepository.GetByIdAsync(rateId);

            if (rate == null)
            {
                throw new NotFoundException("Bài đánh giá không tồn tại");
            }

            var existingRateLike = await _rateLikeRepository.GetByIdAsync(rateId, userId);

            if (existingRateLike == null)
            {
                // Tạo mới RateLike và cập nhật lượt thích/không thích
                var rateLike = new RateLike
                {
                    RateId = rateId,
                    UserId = userId,
                    IsLike = isLike
                };

                await _rateLikeRepository.AddAsync(rateLike);
                return await UpdateLikeAsync(rateId, true, isLike ? "like" : "dislike");
            }
            else
            {
                // Kiểm tra nếu giá trị IsLike thay đổi
                if (existingRateLike.IsLike != isLike)
                {
                    // Cập nhật giá trị IsLike
                    var previousType = existingRateLike.IsLike ? "like" : "dislike";
                    existingRateLike.IsLike = isLike;
                    await _rateLikeRepository.UpdateAsync(existingRateLike);

                    // Cập nhật lượt thích/không thích
                    return await UpdateLikeAsync(rateId, true, isLike ? "like" : "dislike", previousType);
                }
                else
                {
                    // Nếu giá trị IsLike không thay đổi, xóa RateLike (hủy thích/không thích)
                    await _rateLikeRepository.DeleteAsync(existingRateLike.Id);

                    // Cập nhật lượt thích/không thích
                    return await UpdateLikeAsync(rateId, false, isLike ? "like" : "dislike");
                }
            }
        }




        public async Task<(int likeCount, int dislikeCount)> UpdateLikeAsync(long id, bool isIncrement, string type = "like", string preType = null)
        {
            var existingRate = await _rateRepository.GetByIdAsync(id);

            if (existingRate == null)
            {
                throw new NotFoundException("Bài đánh giá không tồn tại");
            }

            if (preType == null)
            {
                if (type == "like")
                {
                    existingRate.Like = isIncrement ? existingRate.Like + 1 : existingRate.Like - 1;
                }
                else if (type == "dislike")
                {
                    existingRate.Dislike = isIncrement ? existingRate.Dislike + 1 : existingRate.Dislike - 1;
                }
            }
            else
            {
                if (type == "like")
                {
                    if (isIncrement)
                    {
                        existingRate.Like++;
                        if (preType == "dislike")
                        {
                            existingRate.Dislike--;
                        }
                    }
                    else
                    {
                        existingRate.Like--;
                    }
                }
                else if (type == "dislike")
                {
                    if (isIncrement)
                    {
                        existingRate.Dislike++;
                        if (preType == "like")
                        {
                            existingRate.Like--;
                        }
                    }
                    else
                    {
                        existingRate.Dislike--;
                    }
                }
            }

            await _rateRepository.UpdateAsync(existingRate);

            // Trả về số lượng like và dislike mới
            return (existingRate.Like.Value, existingRate.Dislike.Value);
        }



public async Task<RateGetDTO> CreateRateAsync(RateInputDTO rateInputDTO, string token)
{
    var userId = _accountService.ExtractUserIdFromToken(token);
    if (userId == null)
    {
        throw new NotFoundException("Có lỗi xãy ra vui lòng đăng nhập lại");
    }

    var orders = await _orderService.GetReceivedOrderByUserIdAsync(userId);

    // Kiểm tra xem trong danh sách đơn hàng có đơn hàng nào chứa sản phẩm cần đánh giá không
    var isProductInOrders = orders.Any(order => order.OrderDetails.Any(orderDetail => orderDetail.ProductId == rateInputDTO.ProductId));

    // Nếu không có sản phẩm trong đơn hàng, ném ra một ngoại lệ
    if (!isProductInOrders)
    {
        throw new NotFoundException("Mua sản phẩm để đánh giá");
    }

    // Lấy thông tin người dùng
    var user = await _accountService.GetUserByIdAsync(userId);

    // Đếm số lượng đánh giá của người dùng cho sản phẩm này
    var countRateUser = await _rateRepository.CountRateAsync(userId, rateInputDTO.ProductId.Value);

    // Đếm số lượng sản phẩm đã mua để đánh giá
    var countOrder = orders.Sum(order => order.OrderDetails.Count(orderDetail => orderDetail.ProductId == rateInputDTO.ProductId));

    if (countRateUser >= countOrder)
    {
        throw new BadRequestException($"Bạn đã đánh giá {countRateUser}/{countOrder} lần rồi.");
    }

    // Tạo mới đánh giá
    var rate = new Rate
    {
        Star = rateInputDTO.Star,
        Content = rateInputDTO.Content,
        ProductId = rateInputDTO.ProductId,
        Like = 0,
        Dislike = 0,
        Status = 1,
        UserId = user.Id,
    };
    await _rateRepository.AddAsync(rate);

    // Tính toán lại giá trị trung bình của đánh giá (Star) cho sản phẩm
    var allRates = await _rateRepository.GetByProductIdAsync(rateInputDTO.ProductId.Value);
    var averageStar = allRates.Average(r => r.Star);

    // Cập nhật giá trị trung bình của đánh giá (Star) cho sản phẩm
    var product = await _productRepository.GetByIdAsync(rateInputDTO.ProductId.Value);
    if (product == null)
    {
        throw new NotFoundException("Không tìm thấy sản phẩm");
    }
    product.Star = averageStar.Value;
    await _productRepository.UpdateAsync(product);

    return _mapper.Map<RateGetDTO>(rate);
}

public async Task ReportRateAsync(long id){
    var rate = await _rateRepository.GetByIdAsync(id);
    if(rate==null){
        throw new NotFoundException("Bài đánh giá không tồn tại");
    }
    rate.Report++;
    await _rateRepository.UpdateAsync(rate);
}

        public async Task DeleteRateAsync(long id)
        {
            var existingRate = await _rateRepository.GetByIdAsync(id);
            if (existingRate == null)
            {
                throw new NotFoundException("Thương hiệu không tồn tại.");
            }


            await _rateRepository.DeleteAsync(id);
        }
        public async Task DeleteRatesAsync(List<long> ids)
        {
            foreach (var id in ids)
            {
                var existingRate = await _rateRepository.GetByIdAsync(id);
                if (existingRate != null)
                {
                    await _rateRepository.DeleteAsync(id);
                }
            }
        }
        public async Task UpdateRateStatusAsync(long id)
        {
            var existingRate = await _rateRepository.GetByIdAsync(id);

            if (existingRate == null)
            {
                throw new NotFoundException("Thương hiệu không tồn tại");
            }

            // Cập nhật trạng thái mới (nếu hiện tại là 0 thì cập nhật thành 1, và ngược lại)
            existingRate.Status = existingRate.Status == 0 ? 1 : 0;

            await _rateRepository.UpdateAsync(existingRate);
        }


    }
}
