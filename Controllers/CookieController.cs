using Microsoft.AspNetCore.Mvc;

namespace KartRentalCompany.Controllers
{
    public class CookieController : Controller
    {
        public IActionResult SetCookie()
        {
            Response.Cookies.Append("CookieConsent", "CookieValue", new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddDays(7),
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
            });

            return Ok("Cookie set");
        }

        public IActionResult GetCookie()
        {
            if (Request.Cookies.TryGetValue("CookieConsent", out var cookieValue))
            {
                return Ok(cookieValue);
            }
            return BadRequest("Cookie not found");
        }

        public IActionResult DeleteCookie()
        {
            Response.Cookies.Delete("CookieConsent");
            return Ok("Cookie deleted");
        }
    }
}
