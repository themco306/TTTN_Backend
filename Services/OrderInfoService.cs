
using AutoMapper;
using backend.DTOs;
using backend.Exceptions;
using backend.Helper;
using backend.Models;
using backend.Repositories.IRepositories;


namespace backend.Services
{
    public class OrderInfoService
    {
        private readonly IOrderInfoRepository _orderinfoRepository;
        private readonly IMapper _mapper;
        private readonly Generate _generate;
        private readonly AccountService _accountService;

        public OrderInfoService(IOrderInfoRepository orderinfoRepository, IMapper mapper, Generate generate, AccountService accountService)
        {
            _orderinfoRepository = orderinfoRepository;
            _mapper = mapper;
            _generate = generate;
            _accountService = accountService;
        }
        public async Task<List<OrderInfo>> GetAllAsync()
        {
            var orderInfos = await _orderinfoRepository.GetAllAsync();
            return orderInfos;
        }
       

       

        public async Task<OrderInfo> GetByIdAsync(long id)
        {
            var orderinfo = await _orderinfoRepository.GetByIdAsync(id);
            if (orderinfo == null)
            {
                throw new NotFoundException("Thông tin không tồn tại.");
            }
            // orderinfo.User=null;
            return orderinfo;
        }
        public async Task<OrderInfo> CreateAsync(OrderInfo dataInput)
        {
            var existingUser=await _accountService.GetUserByIdAsync(dataInput.UserId);
            if (existingUser==null){
                throw new NotFoundException("Người dùng không tồn tại");
            }
            var orderInfo=new OrderInfo{
                UserId=dataInput.UserId,
                DeliveryAddress=dataInput.DeliveryAddress,
                DeliveryName=dataInput.DeliveryName,
                DeliveryPhone=dataInput.DeliveryPhone
            };
            await _orderinfoRepository.AddAsync(orderInfo);
            orderInfo.User=null;

            return orderInfo;
        }
        public async Task<OrderInfo> UpdateAsync(long id, OrderInfo dataInput)
        {
            var existing = await _orderinfoRepository.GetByIdAsync(id);
            if (existing == null){
                throw new NotFoundException("Thông tin không tồn tại");
            }
            existing.DeliveryAddress=dataInput.DeliveryAddress;
            existing.DeliveryName=dataInput.DeliveryName;
            existing.DeliveryPhone=dataInput.DeliveryPhone;
            await _orderinfoRepository.UpdateAsync(existing);
            return existing;
        }
        public async Task DeleteOrderInfoAsync(long id)
        {
            var existingOrderInfo = await _orderinfoRepository.GetByIdAsync(id);
            if (existingOrderInfo == null)
            {
                throw new NotFoundException("Thông tin không tồn tại.");
            }


            await _orderinfoRepository.DeleteAsync(id);
        }


    }
}
