using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Payment.Momo.Request
{
    public class CreateMoMoPaymentDTO
    {
        public string OrderCode {get;set;}
        public string RedirectUrl {get;set;}
        
    }
}