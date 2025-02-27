
using System.Reflection;
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
            // var claimTypes = new[] { ClaimType.CategoryClaim, ClaimType.ProductClaim, ClaimType.UserClaim, ClaimType.SliderClaim };
            var claimTypes = typeof(backend.Helper.ClaimType)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(fi => fi.IsLiteral && !fi.IsInitOnly)
            .Select(fi => fi.GetRawConstantValue().ToString())
            .ToArray();
            var claimValues = new[] { ClaimValue.Show, ClaimValue.Add, ClaimValue.Delete, ClaimValue.Edit };

            var claims = new List<ClaimDTO>();

            foreach (var claimType in claimTypes)
            {
                claims.Add(new ClaimDTO
                {
                    ClaimType = claimType,
                    ClaimValues = new List<string>(claimValues)
                });
            }

            return Ok(claims);
        }


        }
}
