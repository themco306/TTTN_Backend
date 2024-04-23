using System.ComponentModel.DataAnnotations;
namespace backend.DTOs
{
    public class ClaimDTO
    {
        public string ClaimType {get;set;}
        public List<string> ClaimValues {get;set;}
    }
}