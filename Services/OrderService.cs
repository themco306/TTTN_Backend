
using AutoMapper;
using backend.DTOs;
using backend.Exceptions;
using backend.Helper;
using backend.Models;
using backend.Repositories.IRepositories;


namespace backend.Services
{
    public class OrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly CouponService _couponService;
        private readonly OrderInfoService _orderInfoService;
        private readonly ProductService _productService;
        private readonly IMapper _mapper;
        private readonly Generate _generate;
        private readonly AccountService _accountService;

        public OrderService(IOrderRepository orderRepository, IMapper mapper, Generate generate, AccountService accountService, OrderInfoService orderInfoService, ProductService productService, IOrderDetailRepository orderDetailRepository, CouponService couponService)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _generate = generate;
            _accountService = accountService;
            _orderInfoService = orderInfoService;
            _productService = productService;
            _orderDetailRepository = orderDetailRepository;
            _couponService = couponService;
        }
        public async Task<List<OrderGetDTO>> GetAllAsync()
        {
            var orders = await _orderRepository.GetAllAsync();
            return _mapper.Map<List<OrderGetDTO>>(orders);
        }
        public async Task<OrderGetDTO> GetByIdAsync(long id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                throw new NotFoundException("Đơn hàng không tồn tại.");
            }

            return _mapper.Map<OrderGetDTO>(order);
        }


        public async Task<OrderGetDTO> CreateAsync(OrderInputDTO dataInput, string token)
        {
            var userId = _accountService.ExtractUserIdFromToken(token);
            var existingUser = await _accountService.GetUserByIdAsync(userId);

            await _orderInfoService.GetByIdAsync(dataInput.OrderInfoId);
            var coupon = await _couponService.SubmitCodeAsync(dataInput.Code);
            var totalPrice = 0.0m;
            foreach (var data in dataInput.OrderDetails)
            {
                var product = await _productService.GetProductByIdAsync(data.ProductId);
                if (data.Quantity > product.Quantity)
                {
                    throw new BadRequestException($"Số lượng sản phẩm {product.Name} không phù hợp");
                }
                if (coupon.ApplicableProducts != null)
                {
                    if (coupon.ApplicableProducts.CategoryIds != null)
                    {
                        if (!coupon.ApplicableProducts.CategoryIds.Contains(product.Category.Id))
                        {
                            if (coupon.ApplicableProducts.ProductIds != null)
                            {
                                if (!coupon.ApplicableProducts.ProductIds.Contains(product.Id))
                                {
                                    throw new NotFoundException($"Sản phẩm {product.Name} không nằm trong chương trình khuyến mãi");
                                }
                            }else{
                                 throw new NotFoundException($"Sản phẩm {product.Name} không nằm trong chương trình khuyến mãi");
                            }
                        }
                    }


                }
                totalPrice += data.Quantity * product.SalePrice;
            }
            if(totalPrice<coupon.MinimumOrderValue){
                throw new BadRequestException($"Mua thêm để đủ {(long)coupon.MinimumOrderValue} VND");
            }
            if (coupon.DiscountType == DiscountType.Percentage)
            {
                totalPrice = totalPrice-(totalPrice * coupon.DiscountValue/100);
            }
            if (coupon.DiscountType == DiscountType.FixedAmount)
            {
                totalPrice -= coupon.DiscountValue;
            }
            var expiresAt = DateTime.UtcNow.AddHours(1);
            var order = new Order
            {
                UserId = existingUser.Id,
                OrderInfoId = dataInput.OrderInfoId,
                Code = _generate.GenerateOrderCode(),
                UpdatedById = existingUser.Id,
                Token = Guid.NewGuid().ToString(),
                ExpiresAt = expiresAt,
                Total = totalPrice,
                PaymentType = dataInput.PaymentType,
                Note = dataInput.Note,
                Status = 0
            };
            await _orderRepository.AddAsync(order);
            foreach (var detail in dataInput.OrderDetails)
            {
                var product = await _productService.GetProductByIdAsync(detail.ProductId);
                var OrderDetail = new OrderDetail
                {
                    OrderId = order.Id,
                    Price = product.SalePrice,
                    ProductId = detail.ProductId,
                    Quantity = detail.Quantity,
                    TotalPrice = product.SalePrice * detail.Quantity,
                };
                await _orderDetailRepository.AddAsync(OrderDetail);

            }
            await _productService.UpdateTagProductAsync();
            return _mapper.Map<OrderGetDTO>(order);
        }
        // public async Task<Order> UpdateAsync(long id, Order dataInput)
        // {
        //     var existing = await _orderRepository.GetByIdAsync(id);
        //     if (existing == null){
        //         throw new NotFoundException("Thông tin không tồn tại");
        //     }
        //     existing.DeliveryAddress=dataInput.DeliveryAddress;
        //     existing.DeliveryName=dataInput.DeliveryName;
        //     existing.DeliveryPhone=dataInput.DeliveryPhone;
        //     await _orderRepository.UpdateAsync(existing);
        //     return existing;
        // }
        // public async Task DeleteOrderAsync(long id)
        // {
        //     var existingOrder = await _orderRepository.GetByIdAsync(id);
        //     if (existingOrder == null)
        //     {
        //         throw new NotFoundException("Thông tin không tồn tại.");
        //     }


        //     await _orderRepository.DeleteAsync(id);
        // }


    }
}
