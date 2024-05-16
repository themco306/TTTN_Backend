
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
        public async Task<List<OrderInfo>> GetAllAsync(string token)
        {
            var userId = _accountService.ExtractUserIdFromToken(token);
            var user =await _accountService.GetUserByIdAsync(userId);
            var orderInfos = await _orderinfoRepository.GetAllAsync(user.Id);
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
        public async Task<OrderInfo> CreateAsync(OrderInfoInputDTO dataInput,string token)
        {
            var userId=_accountService.ExtractUserIdFromToken(token);
            var existingUser=await _accountService.GetUserByIdAsync(userId);
            if (existingUser==null){
                throw new NotFoundException("Người dùng không tồn tại");
            }
            var orderInfos = await _orderinfoRepository.GetAllAsync(existingUser.Id);
            if(orderInfos.Count>3){
                throw new BadRequestException("Bạn chỉ có thể tạo tối đa 4.");
            }
            var orderInfo=new OrderInfo{
                UserId=existingUser.Id,
                DeliveryAddress=dataInput.DeliveryAddress,
                DeliveryProvince=dataInput.DeliveryProvince,
                DeliveryDistrict=dataInput.DeliveryDistrict,
                DeliveryWard=dataInput.DeliveryWard,
                DeliveryName=dataInput.DeliveryName,
                DeliveryPhone=dataInput.DeliveryPhone
            };
            await _orderinfoRepository.AddAsync(orderInfo);
            orderInfo.User=null;

            return orderInfo;
        }
        public async Task<OrderInfo> UpdateAsync(long id, OrderInfoInputDTO dataInput,string token)
        {
            var existing = await _orderinfoRepository.GetByIdAsync(id);
            if (existing == null){
                throw new NotFoundException("Thông tin không tồn tại");
            }
            var userId=_accountService.ExtractUserIdFromToken(token);
            if(userId!=existing.UserId){
                throw new NotFoundException("Thông tin không tồn tại");
            }
            existing.DeliveryAddress=dataInput.DeliveryAddress;
            existing.DeliveryProvince=dataInput.DeliveryProvince;
            existing.DeliveryDistrict=dataInput.DeliveryDistrict;
            existing.DeliveryWard=dataInput.DeliveryWard;
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
