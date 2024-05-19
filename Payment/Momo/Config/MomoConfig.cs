using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Payment.Momo.Config
{
    public class MomoConfig
    {
        public  string PartnerCode {get;set;}=string.Empty;
        public  string IpnUrl {get;set;}=string.Empty;
        public  string AccessKey {get;set;}=string.Empty;
        public  string ScretKey {get;set;}=string.Empty;
        public  string PaymentUrl {get;set;}=string.Empty;
        
    }
}