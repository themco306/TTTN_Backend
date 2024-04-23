
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace backend.Models
{
    public class AppUser :IdentityUser
    {
        public string FirstName {get;set;}=null!;

        public string LastName {get;set;}=null!;

        public string? Avatar {get;set;}

        public bool? Gender {get;set;}//true-nam;false-nữ
    }
}