using KartRentalCompany.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace KartRentalCompany.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;

        public AdminController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        // GET: Admins
        [HttpGet]
        public IActionResult ManageAdmins()
        {
            return View(new AdminViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole(AdminViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return NotFound($"User with email {model.Email} could not be found");
            }
            var result = await _userManager.AddToRoleAsync(user, model.Role);
            if (result.Succeeded)
            {
                return Ok($"User {model.Email} was successfully assigned to role {model.Role}.");
            }
            return BadRequest(result.Errors);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveRole(AdminViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return NotFound($"User with email {model.Email} could not be found");
            }
            var result = await _userManager.RemoveFromRoleAsync(user, model.Role);
            if (result.Succeeded)
            {
                return Ok($"User {model.Email} was successfully removed from role {model.Role}.");
            }
            return BadRequest(result.Errors);
        }
    }
}

/* ToDo: Add role handling through the views
 * Add a view with a reservation callendar, admin restricted */