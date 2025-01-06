using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<IActionResult> Index()
    {
        var adminRole = await _roleManager.FindByNameAsync("Admin");
        if (adminRole == null)
        {
            return View(new List<IdentityUser>());
        }

        var admins = await _userManager.GetUsersInRoleAsync("Admin");
        return View(admins);
    }

    [HttpPost]
    public async Task<IActionResult> AddAdmin(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            TempData["ErrorMessage"] = $"No user found with the email '{email}'.";
            return RedirectToAction(nameof(Index));
        }

        var result = await _userManager.AddToRoleAsync(user, "Admin");
        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = $"{user.UserName} ({email}) has been added to the Admin role.";
        }
        else
        {
            TempData["ErrorMessage"] = $"Failed to add {email} to the Admin role: {string.Join(", ", result.Errors.Select(e => e.Description))}";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> RemoveAdmin(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound(); 
        }

        var result = await _userManager.RemoveFromRoleAsync(user, "Admin");
        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = $"{user.UserName} has been removed from the Admin role.";
        }
        else
        {
            TempData["ErrorMessage"] = $"Failed to remove {user.UserName} from the Admin role: {string.Join(", ", result.Errors.Select(e => e.Description))}";
        }

        return RedirectToAction(nameof(Index));
    }
}
