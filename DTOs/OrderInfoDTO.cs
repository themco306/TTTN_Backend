
using System.ComponentModel.DataAnnotations;
namespace backend.DTOs
{
 public class OrderInfoInputDTO{
     public string DeliveryAddress {get;set;}
       
        public string DeliveryProvince {get;set;}
        public string DeliveryDistrict {get;set;}
         public string DeliveryWard {get;set;}

        public string DeliveryName {get;set;}
        public string DeliveryPhone {get;set;}
 }

}