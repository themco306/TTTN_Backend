
using backend.DTOs;
using backend.Helper;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
        [ApiController]
        [Route("api/claims")]

        public class ClaimsController : ControllerBase
        {
            [HttpGet]
        public ActionResult GetClaims()
        {
            var claims = new List<ClaimDTO>
            {
                new ClaimDTO
                {
                    ClaimType = ClaimType.CategoryClaim,
                    ClaimValues = new List<string> {ClaimValue.Show, ClaimValue.Add, ClaimValue.Delete,ClaimValue.Edit }
                },
                new ClaimDTO
                {
                    ClaimType = ClaimType.ProductClaim,
                    ClaimValues = new List<string> {ClaimValue.Show, ClaimValue.Add, ClaimValue.Delete,ClaimValue.Edit }
                },
                                new ClaimDTO
                {
                    ClaimType = ClaimType.UserClaim,
                    ClaimValues = new List<string> {ClaimValue.Show, ClaimValue.Add, ClaimValue.Delete,ClaimValue.Edit }
                },
                
            };

            return Ok(claims);
        }


        }
}
