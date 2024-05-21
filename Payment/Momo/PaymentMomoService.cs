
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using backend.Exceptions;
using backend.Models;
using backend.Payment.Momo.Config;
using backend.Payment.Momo.Request;
using backend.Payment.Momo.Response;
using backend.Repositories.IRepositories;
using backend.Services;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using Newtonsoft.Json;

namespace backend.Payment.Momo
{
    public class PaymentMomoService
    {
        private readonly IOptions<MomoConfig> _configMomo;
        private readonly OrderService _orderService;
        private readonly IPaidOrderRepository _paidOrderRepository;
        private static readonly HttpClient client = new HttpClient();

        public PaymentMomoService(IOptions<MomoConfig> configMomo, OrderService orderService,IPaidOrderRepository paidOrderRepository)
        {
            _configMomo = configMomo;
            _orderService = orderService;
            _paidOrderRepository=paidOrderRepository;
        }
        public static string GenerateSignature(string rawData, string secretKey)
        {
            byte[] secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);
            byte[] dataBytes = Encoding.UTF8.GetBytes(rawData);

            using (HMACSHA256 hmac = new HMACSHA256(secretKeyBytes))
            {
                byte[] hashedBytes = hmac.ComputeHash(dataBytes);
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
        public async Task<MoMoPaymentResponse> GetLinkPayment(CreateMoMoPaymentDTO moMoPaymentDTO, string token)
        {

            var order = await _orderService.GetByCodeAsync(moMoPaymentDTO.OrderCode, token);
            if(order.PaymentType!=PaymentType.OnlinePayment){
                throw new Exception("Đơn hàng này không được thanh toán trực tuyến");
            }
            var exittingPaidOrder= await _paidOrderRepository.GetByOrderCodeAsync(order.Code);
            Guid myuuid = Guid.NewGuid();
            string myuuidAsString = myuuid.ToString();
            if (exittingPaidOrder == null){
                var paidOrder=new PaidOrder{
                    OrderId=order.Id,
                    PaymentMethodCode=myuuidAsString,
                    PaymentMethod=PaymentMethod.MomoPayment,
                    Amount=order.Total,
                };
                await _paidOrderRepository.AddAsync(paidOrder);
            }else{
                if(exittingPaidOrder.PaymentDate!=null){
                    throw new BadRequestException("Đơn hàng này đã được thanh toán");
                }
                exittingPaidOrder.PaymentMethodCode=myuuidAsString;
                await _paidOrderRepository.UpdateAsync(exittingPaidOrder);
            }
            string accessKey = _configMomo.Value.AccessKey;
            string secretKey = _configMomo.Value.ScretKey;
            string partnerCode=_configMomo.Value.PartnerCode;
            string paymentUrl=_configMomo.Value.PaymentUrl;
            string ipnUrl=_configMomo.Value.IpnUrl;

            

            MoMoPaymentRequest request = new MoMoPaymentRequest();
            request.orderInfo ="Mã Tk Shop: " +order.Code;
            request.partnerCode = partnerCode;
            request.ipnUrl = ipnUrl;
            request.redirectUrl = moMoPaymentDTO.RedirectUrl;
            request.amount = (long)order.Total;
            request.orderId = myuuidAsString;
            request.requestId = myuuidAsString;
            request.requestType = "payWithMethod";
            request.extraData = "";
            request.partnerName = "Shop TK";
            request.storeId = "Test Store";
            request.orderGroupId = "";
            request.autoCapture = true;
            request.lang = "vi";

            var rawSignature = "accessKey=" + accessKey + "&amount=" + request.amount + "&extraData=" + request.extraData + "&ipnUrl=" + request.ipnUrl + "&orderId=" + request.orderId + "&orderInfo=" + request.orderInfo + "&partnerCode=" + request.partnerCode + "&redirectUrl=" + request.redirectUrl + "&requestId=" + request.requestId + "&requestType=" + request.requestType;

            request.signature = GenerateSignature(rawSignature, secretKey);

            StringContent httpContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json");
            var quickPayResponse = await client.PostAsync(paymentUrl, httpContent);
            var contents = quickPayResponse.Content.ReadAsStringAsync().Result;
            var response=JsonConvert.DeserializeObject<MoMoPaymentResponse>(contents);
            if(response.resultCode!=0){
               throw new BadRequestException(response.message);
            }
            return response;
        }
        
        public async Task<bool> PaymentSuccess(MoMoSuccessDTO response){
            var now = DateTime.UtcNow;
             var exittingPaidOrder= await _paidOrderRepository.GetByPaymentMethodCodeAsync(response.orderId);
             if(exittingPaidOrder==null){
                throw new BadRequestException("Có lỗi xảy ra vui lòng liên hệ với nhân viên để được hoàn tiền");
             }
            var order = await _orderService.GetByIdAsync(exittingPaidOrder.OrderId);
            if((long)order.Total!=response.amount){
                throw new BadRequestException("Bạn thanh toán số tền không hợp lệ");
            }

            string accessKey = _configMomo.Value.AccessKey;
            string secretKey = _configMomo.Value.ScretKey;
            var rawSignature = "accessKey=" + accessKey + "&amount=" + response.amount + "&extraData=" + response.extraData + "&message=" + response.message + "&orderId=" + response.orderId + "&orderInfo=" + response.orderInfo + "&orderType=" + response.orderType + "&partnerCode=" + response.partnerCode + "&payType=" + response.payType + "&requestId=" + response.requestId + "&responseTime=" + response.responseTime + "&resultCode=" + response.resultCode + "&transId=" + response.transId;
 
            string signature = GenerateSignature(rawSignature, secretKey);
            if (signature != response.signature)
            {
                throw new BadRequestException("Có lỗi xảy ra");
            }

            if (response.resultCode == 0)
            {
                exittingPaidOrder.PaymentDate=now;
                await _paidOrderRepository.UpdateAsync(exittingPaidOrder);
                await _orderService.UpdatePaymentStatusAsync(order.Id,OrderStatus.PaymentCompleted,order.User.Id);
               return true;
            }
            else
            {
                throw new BadRequestException(response.message);
            }
        }
    }

}