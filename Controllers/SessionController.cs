using Microsoft.AspNetCore.Mvc;

namespace KartRentalCompany.Controllers
{
    public class SessionController : Controller
    {
        public IActionResult SetSession()
        {
            HttpContext.Session.SetString("SessionKey", "SessionValue");
            HttpContext.Session.SetInt32("SessionNumber", 12345);

            return Content("Session data has been set!");
        }
        public IActionResult GetSession()
        {
            var sessionValue = HttpContext.Session.GetString("SessionKey");
            var sessionNumber = HttpContext.Session.GetInt32("SessionNumber");

            return Content($"Session Value = {sessionValue}, Session Number = {sessionNumber}");
        }
        public IActionResult DeleteSession()
        {
            HttpContext.Session.Remove("SessionKey");
            return Content("Session data deleted");
        }
    }
}
