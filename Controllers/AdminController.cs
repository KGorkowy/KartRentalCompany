using KartRentalCompany.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace KartRentalCompany.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;

        public AdminController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole(string email, string role)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound($"User with email {email} could not be found");
            }
            var result = await _userManager.AddToRoleAsync(user, role);
            if (result.Succeeded)
            {
                return Ok($"User {email} was successfully assigned to role {role}.");
            }
            return BadRequest(result.Errors);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult AdminOnly()
        {
            return Ok("You are an admin!");
        }

        [HttpGet]
        public async Task<IActionResult> IsUserAdmin(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound($"User with email {email} could not be found");
            }
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            return Ok(new { Email = email, IsAdmin = isAdmin });
        }
    }
}