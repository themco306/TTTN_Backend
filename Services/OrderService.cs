
using AutoMapper;
using backend.DTOs;
using backend.Exceptions;
using backend.Helper;
using backend.Models;
using backend.Repositories;
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
        private readonly EmailService _emailService;
        private readonly ICartItemRepository _cartItemRepository;
        private readonly ICouponUsageRepository _couponUsageRepository;

        public OrderService(IOrderRepository orderRepository, IMapper mapper, Generate generate, AccountService accountService, OrderInfoService orderInfoService, ProductService productService, IOrderDetailRepository orderDetailRepository, CouponService couponService, EmailService emailService, ICartItemRepository cartItemRepository, ICouponUsageRepository couponUsageRepository)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _generate = generate;
            _accountService = accountService;
            _orderInfoService = orderInfoService;
            _productService = productService;
            _orderDetailRepository = orderDetailRepository;
            _couponService = couponService;
            _emailService = emailService;
            _cartItemRepository = cartItemRepository;
            _couponUsageRepository = couponUsageRepository;
        }
public async Task<OrderSummaryDTO> GetOrderSummaryDashboardAsync(DatetimeQueryDTO datetimeQuery)
{
    var orderSummary = new OrderSummaryDTO();
    
    // Get the current DateTimeOffset with the correct time zone
    DateTimeOffset now = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(+7)); // Adjust for Vietnam time zone
    
    // Set default values for start date and end date if they are null
    if (datetimeQuery.StartDate == DateTimeOffset.MinValue)
    {
        datetimeQuery.StartDate = new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0, now.Offset);
    }

    if (datetimeQuery.EndDate == DateTimeOffset.MinValue)
    {
        datetimeQuery.EndDate = now.AddDays(1);
    }

    var statuses = new List<OrderStatus> { OrderStatus.Received, OrderStatus.Delivered };

    // Call the method from repository to get order data based on datetimeQuery
    var orders = await _orderRepository.GetOrdersBetweenDatesAsync(datetimeQuery.StartDate.DateTime, datetimeQuery.EndDate.DateTime, statuses);

    // Calculate summary information about orders
    orderSummary.TotalOrders = orders.Count();
    orderSummary.TotalRevenue = CalculateTotalRevenue(orders);
    orderSummary.AverageOrderValue = CalculateAverageOrderValue(orders);

    // Ensure that orderSummary has been filled with complete information
    return orderSummary;
}


private decimal CalculateTotalRevenue(IEnumerable<Order> orders)
{
    // Tính tổng doanh thu từ các đơn hàng
    decimal totalRevenue = orders.Sum(order => order.Total);
    return totalRevenue;
}

private decimal CalculateAverageOrderValue(IEnumerable<Order> orders)
{
    // Tính giá trị trung bình của mỗi đơn hàng
    if (orders.Any())
    {
        decimal totalRevenue = CalculateTotalRevenue(orders);
        int totalOrders = orders.Count();
        decimal averageOrderValue = totalRevenue / totalOrders;
        return averageOrderValue;
    }
    else
    {
        // Trường hợp không có đơn hàng, trả về 0
        return 0;
    }
}

public async Task<Dictionary<OrderStatus, int>> GetStatusOrderDashboardAsync(string timeFrame)
{
    var orderStatusCounts = new Dictionary<OrderStatus, int>();

    // Get all orders
    var orders = await _orderRepository.GetAllAsync();

    // Filter orders based on time frame
    DateTime now = DateTime.UtcNow;
    DateTime startDate;

    switch (timeFrame.ToLower())
    {
        case "today":
            startDate = now.Date;
            break;
        case "thismonth":
            startDate = new DateTime(now.Year, now.Month, 1);
            break;
        case "all":
        default:
            startDate = DateTime.MinValue;
            break;
    }

    var filteredOrders = orders.Where(o => o.CreatedAt >= startDate).ToList();

    // Count orders by status
    foreach (OrderStatus status in Enum.GetValues(typeof(OrderStatus)))
    {
        orderStatusCounts[status] = filteredOrders.Count(o => o.Status == status);
    }

    return orderStatusCounts;
}


        public async Task<List<OrderGetDTO>> GetAllAsync()
        {
            var orders = await _orderRepository.GetAllAsync();
            return _mapper.Map<List<OrderGetDTO>>(orders);
        }
        public async Task<List<OrderGetReceivedDTO>> GetReceivedOrderByUserIdAsync(string userId)
        {
            var orders = await _orderRepository.GetReceivedOrderByUserIdAsync(userId);
            return _mapper.Map<List<OrderGetReceivedDTO>>(orders);
        }
        public async Task<PagedResult<OrderGetDTO>> GetMyOrdersAsync(string token, int page, int pageSize)
        {
            var userId = _accountService.ExtractUserIdFromToken(token);
            var existingUser = await _accountService.GetUserByIdAsync(userId);
            var totalItem = await _orderRepository.GetTotalOrderCountAsync(existingUser.Id);
            var orders = await _orderRepository.GetMyOrdersAsync(existingUser.Id, page, pageSize);
            var orderGetDTOs = _mapper.Map<List<OrderGetDTO>>(orders);

            return new PagedResult<OrderGetDTO>
            {
                Items = orderGetDTOs,
                TotalCount = totalItem,
                PageSize = pageSize,
                CurrentPage = page
            };
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
        public async Task<OrderWithCouponDTO> GetByIdSAsync(long id)
        {

            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                throw new NotFoundException("Đơn hàng không tồn tại.");
            }
            var couponU = await _couponUsageRepository.GetByOrderIdAsync(id);


            var orderGet = _mapper.Map<OrderGetDTO>(order);
            return new OrderWithCouponDTO
            {
                CouponUsage = couponU != null ? couponU.Coupon.Description : null,
                Order = orderGet
            };
        }
        public async Task UpdateStatusAsync(long id, OrderStatus status, string token)
        {
            var userId = _accountService.ExtractUserIdFromToken(token);
            var existingUser = await _accountService.GetUserByIdAsync(userId);
            var order = await _orderRepository.GetByIdAsync(id);
            switch (status)
            {
                case OrderStatus.Cancelled:
                    {
                        if (order.Status != OrderStatus.Received)
                        {
                            order.Status = OrderStatus.Cancelled;
                            order.UpdatedById = existingUser.Id;
                            await _orderRepository.UpdateAsync(order);
                        }
                    }
                    break;
                case OrderStatus.PaymentCompleted:
                    {
                        if (order.PaymentType == PaymentType.OnlinePayment && order.Status == OrderStatus.Confirmed)
                        {
                            order.Status = OrderStatus.PaymentCompleted;
                            order.UpdatedById = existingUser.Id;
                            await _orderRepository.UpdateAsync(order);
                        }
                    }
                    break;
                case OrderStatus.Shipped:
                    {
                        if (order.PaymentType == PaymentType.OnlinePayment && order.Status == OrderStatus.PaymentCompleted)
                        {
                            order.Status = OrderStatus.Shipped;
                            order.UpdatedById = existingUser.Id;
                            await _orderRepository.UpdateAsync(order);
                        }
                        if (order.PaymentType == PaymentType.CashOnDelivery && order.Status == OrderStatus.Confirmed)
                        {
                            order.Status = OrderStatus.Shipped;
                            order.UpdatedById = existingUser.Id;
                            await _orderRepository.UpdateAsync(order);
                        }
                    }
                    break;
                case OrderStatus.Delivered:
                    {
                        if (order.Status == OrderStatus.Shipped)
                        {
                            order.Status = OrderStatus.Delivered;
                            order.UpdatedById = existingUser.Id;
                            await _orderRepository.UpdateAsync(order);
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        public async Task UpdateMyOrderStatusAsync(long id, OrderStatus status, string token)
        {
            var userId = _accountService.ExtractUserIdFromToken(token);
            var existingUser = await _accountService.GetUserByIdAsync(userId);
            var order = await _orderRepository.GetByIdAsync(id);
            if (userId != order.UserId)
            {
                throw new BadRequestException("Đơn hàng này không phải của bạn");
            }
            switch (status)
            {
                case OrderStatus.Cancelled:
                    {
                        if (order.Status != OrderStatus.Received)
                        {
                            order.Status = OrderStatus.Cancelled;
                            order.UpdatedById = existingUser.Id;
                            await _orderRepository.UpdateAsync(order);
                        }
                    }
                    break;

                case OrderStatus.Received:
                    {
                        if (order.Status == OrderStatus.Delivered)
                        {
                            order.Status = OrderStatus.Received;
                            order.UpdatedById = existingUser.Id;
                            await _orderRepository.UpdateAsync(order);
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        public async Task UpdatePaymentStatusAsync(long id, OrderStatus status, string userId)
        {
            var existingUser = await _accountService.GetUserByIdAsync(userId);
            var order = await _orderRepository.GetByIdAsync(id);
            switch (status)
            {
                case OrderStatus.Cancelled:
                    {
                        if (order.Status != OrderStatus.Received)
                        {
                            order.Status = OrderStatus.Cancelled;
                            order.UpdatedById = existingUser.Id;
                            await _orderRepository.UpdateAsync(order);
                        }
                    }
                    break;
                case OrderStatus.PaymentCompleted:
                    {
                        if (order.PaymentType == PaymentType.OnlinePayment && order.Status == OrderStatus.Confirmed)
                        {
                            order.Status = OrderStatus.PaymentCompleted;
                            order.UpdatedById = existingUser.Id;
                            await _orderRepository.UpdateAsync(order);
                        }
                    }
                    break;
                case OrderStatus.Shipped:
                    {
                        if (order.PaymentType == PaymentType.OnlinePayment && order.Status == OrderStatus.PaymentCompleted)
                        {
                            order.Status = OrderStatus.Shipped;
                            order.UpdatedById = existingUser.Id;
                            await _orderRepository.UpdateAsync(order);
                        }
                        if (order.PaymentType == PaymentType.CashOnDelivery && order.Status == OrderStatus.Confirmed)
                        {
                            order.Status = OrderStatus.Shipped;
                            order.UpdatedById = existingUser.Id;
                            await _orderRepository.UpdateAsync(order);
                        }
                    }
                    break;
                case OrderStatus.Delivered:
                    {
                        if (order.Status == OrderStatus.Shipped)
                        {
                            order.Status = OrderStatus.Delivered;
                            order.UpdatedById = existingUser.Id;
                            await _orderRepository.UpdateAsync(order);
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        public async Task<OrderWithCouponDTO> GetByCodeAsync(string code, string token)
        {
            var userId = _accountService.ExtractUserIdFromToken(token);
            var existingUser = await _accountService.GetUserByIdAsync(userId);
            var order = await _orderRepository.GetByCodeAsync(code);
            if (order == null)
            {
                throw new NotFoundException("Đơn hàng không tồn tại.");
            }
            if (existingUser.Id != order.UserId)
            {
                throw new NotFoundException("Bạn không thể xem đơn hàng của người khác");
            }
            var couponU = await _couponUsageRepository.GetByOrderIdAsync(order.Id);


            var orderGet = _mapper.Map<OrderGetDTO>(order);
            return new OrderWithCouponDTO
            {
                CouponUsage = couponU != null ? couponU.Coupon.Description : null,

                Order = orderGet
            };
        }
        public async Task SendEmailConfirm(long id, string url, string token)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                throw new NotFoundException("Đơn hàng  không tồn tại");
            }
            var userId = _accountService.ExtractUserIdFromToken(token);
            var existingUser = await _accountService.GetUserByIdAsync(userId);
            var encodedToken = System.Net.WebUtility.UrlEncode(order.Token);
            var confirmationLink = $"{url}/{order.Code}/{encodedToken}";
            var html = TableUtils.CreateHtmlTable(order, confirmationLink);
            await _emailService.SendEmailAsync(existingUser.Email, "Xác nhận đơn hàng của bạn", html);
        }
        public async Task<bool> ConfirmEmailAsync(string userId, string confirmEmailToken)
        {

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(confirmEmailToken))
            {
                throw new BadRequestException("Đường đẫn có vấn đề.");
            }

            var order = await _orderRepository.GetByCodeAsync(userId);
            if (order == null)
            {
                throw new NotFoundException("Đơn hàn không tồn tại hoặc đường đẫn có vấn đề.");
            }
            var now = DateTime.UtcNow;
            if (order.ExpiresAt <= now)
            {
                throw new NotFoundException("Liên kết hết hiệu lực, vui lòng hủy đơn hàng.");
            }
            if (order.Token != confirmEmailToken)
            {
                throw new NotFoundException("Đơn hàn không tồn tại hoặc đường đẫn có vấn đề.");
            }
            order.Status = OrderStatus.Confirmed;
            await _orderRepository.UpdateAsync(order);
            return true;
        }
        public async Task<OrderGetDTO> CreateAsync(OrderInputDTO dataInput, string token)
        {
            var userId = _accountService.ExtractUserIdFromToken(token);
            var existingUser = await _accountService.GetUserByIdAsync(userId);

            await _orderInfoService.GetByIdAsync(dataInput.OrderInfoId);

            CouponGetDTO coupon = null;
            if (!string.IsNullOrEmpty(dataInput.Code))
            {
                coupon = await _couponService.SubmitCodeAsync(dataInput.Code);
                var couponUsage = await _couponUsageRepository.GetByIdAsync(existingUser.Id, coupon.Id);
                if (couponUsage.Count >= coupon.UsagePerUser)
                {
                    throw new BadRequestException($"Số lần bạn dùng Mã này đã là {couponUsage.Count}/{coupon.UsagePerUser}");
                }
            }

            var totalPrice = 0.0m;
            foreach (var data in dataInput.OrderDetails)
            {
                var product = await _productService.GetProductByIdAsync(data.ProductId);
                if (data.Quantity > product.Quantity)
                {
                    throw new BadRequestException($"Số lượng sản phẩm {product.Name} không phù hợp");
                }

                if (coupon != null && coupon.ApplicableProducts != null)
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
                            }
                            else
                            {
                                throw new NotFoundException($"Sản phẩm {product.Name} không nằm trong chương trình khuyến mãi");
                            }
                        }
                    }
                }

                totalPrice += data.Quantity * product.SalePrice;
            }

            if (coupon != null)
            {
                if (totalPrice < coupon.MinimumOrderValue)
                {
                    throw new BadRequestException($"Mua thêm để đủ {(long)coupon.MinimumOrderValue} VND");
                }

                if (coupon.DiscountType == DiscountType.Percentage)
                {
                    totalPrice = totalPrice - (totalPrice * coupon.DiscountValue / 100);
                }
                else if (coupon.DiscountType == DiscountType.FixedAmount)
                {
                    totalPrice -= coupon.DiscountValue;
                }
            }

            var expiresAt = DateTime.UtcNow;
            var order = new Order
            {
                UserId = existingUser.Id,
                OrderInfoId = dataInput.OrderInfoId,
                Code = _generate.GenerateOrderCode(),
                UpdatedById = existingUser.Id,
                Token = Guid.NewGuid().ToString(),
                ExpiresAt = expiresAt.AddMinutes(5),
                Total = totalPrice,
                PaymentType = dataInput.PaymentType,
                Note = dataInput.Note,
                Status = 0
            };
            await _orderRepository.AddAsync(order);

            foreach (var detail in dataInput.OrderDetails)
            {
                var product = await _productService.GetProductByIdAsync(detail.ProductId);
                var orderDetail = new OrderDetail
                {
                    OrderId = order.Id,
                    Price = product.SalePrice,
                    ProductId = detail.ProductId,
                    Quantity = detail.Quantity,
                    TotalPrice = product.SalePrice * detail.Quantity,
                };
                await _productService.UpdateProductQuantityAsync(product.Id, detail.Quantity);
                await _orderDetailRepository.AddAsync(orderDetail);
                await _cartItemRepository.DeleteAsync(detail.CartId);
            }
            if (coupon != null)
            {
                var couponUsage = new CouponUsage
                {
                    UserId = existingUser.Id,
                    CouponId = coupon.Id,
                    OrderId = order.Id,
                    UsedAt = expiresAt
                };
                // await _couponService.UpdateCouponUsageLimitAsync(coupon.Id,coupon.UsageLimit-1);
                await _couponUsageRepository.AddAsync(couponUsage);
            }

            await _productService.UpdateTagProductAsync();

            return _mapper.Map<OrderGetDTO>(order);
        }


        // public async Task<> SendEmailConfirmOrder (){

        // }
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
