
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
        private readonly OrderInfoService _orderInfoService;
        private readonly ProductService _productService;
        private readonly IMapper _mapper;
        private readonly Generate _generate;
        private readonly AccountService _accountService;

        public OrderService(IOrderRepository orderRepository, IMapper mapper, Generate generate, AccountService accountService,OrderInfoService orderInfoService,ProductService productService,IOrderDetailRepository orderDetailRepository)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _generate = generate;
            _accountService = accountService;
            _orderInfoService = orderInfoService;
            _productService=productService;
            _orderDetailRepository=orderDetailRepository;
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


        public async Task<Order> CreateAsync(OrderInputDTO dataInput)
        {
             await _accountService.GetUserByIdAsync(dataInput.UserId);
            await _orderInfoService.GetByIdAsync(dataInput.OrderInfoId);
            foreach(var data in dataInput.OrderDetails){

            var product=await _productService.GetProductByIdAsync(data.ProductId);
            if(data.Quantity>product.Quantity){
                throw new BadRequestException("Số lượng không phù hợp");
            }
            }

            var order=new Order{
                UserId=dataInput.UserId,
                OrderInfoId=dataInput.OrderInfoId,
                Code=_generate.GenerateOrderCode(),
                UpdatedById=dataInput.UserId,
                Status=0
            };
            await _orderRepository.AddAsync(order);
            foreach(var detail in dataInput.OrderDetails){
                var product=await _productService.GetProductByIdAsync(detail.ProductId);
                var OrderDetail=new OrderDetail{
                    OrderId=order.Id,
                    Price=product.SalePrice,
                    ProductId=detail.ProductId,
                    Quantity=detail.Quantity,
                    TotalPrice=product.SalePrice*detail.Quantity,
                };
                await _orderDetailRepository.AddAsync(OrderDetail);

            }
            return order;
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
