
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace backend.Models
{
    [Keyless]
    public class AppUser :IdentityUser
    {
        public string FirstName {get;set;}=null!;

        public string LastName {get;set;}=null!;
    }
}