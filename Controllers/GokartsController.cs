using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KartRentalCompany.Data;
using KartRentalCompany.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;

namespace KartRentalCompany.Controllers
{
    [Authorize]
    public class GokartsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GokartsController> _logger;

        public GokartsController(ApplicationDbContext context, ILogger<GokartsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Gokarts
        public async Task<IActionResult> Index()
        {
            return View(await _context.Gokart.ToListAsync());
        }

        // GET: Gokarts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gokart = await _context.Gokart
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gokart == null)
            {
                return NotFound();
            }

            return View(gokart);
        }

        // GET: Gokarts/Create
        [Authorize(Policy = "RequireAdminRole")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Gokarts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> Create([Bind("Name,Manufacturer,Price,PricePerDay,EngineSize,Description,ImageUrl")] Gokart gokart)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Call the stored procedure
                    await _context.Database.ExecuteSqlRawAsync(
                        "EXEC AddGokart @Manufacturer, @Name, @Price, @PricePerDay, @EngineSize, @Description, @ImageUrl",
                        new SqlParameter("@Manufacturer", gokart.Manufacturer),
                        new SqlParameter("@Name", gokart.Name),
                        new SqlParameter("@Price", gokart.Price),
                        new SqlParameter("@PricePerDay", gokart.PricePerDay),
                        new SqlParameter("@EngineSize", gokart.EngineSize),
                        new SqlParameter("@Description", gokart.Description),
                        new SqlParameter("@ImageUrl", gokart.ImageUrl)
                    );

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Log the error and show a user-friendly message
                    _logger.LogError(ex, "Error occurred while creating a Gokart using the stored procedure.");
                    ModelState.AddModelError(string.Empty, "An error occurred while saving. Please try again.");
                }
            }

            return View(gokart);
        }

        // GET: Gokarts/Edit/5
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gokart = await _context.Gokart.FindAsync(id);
            if (gokart == null)
            {
                return NotFound();
            }
            return View(gokart);
        }

        // POST: Gokarts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Manufacturer,Price,PricePerDay,EngineSize,Description,ImageUrl")] Gokart gokart)
        {
            if (id != gokart.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Entry(gokart).Property(x => x.PricePerDay).IsModified = true;
                    await _context.Database.ExecuteSqlRawAsync(
                        "UPDATE Gokart SET Description = {0}, EngineSize = {1}, ImageUrl = {2}, Manufacturer = {3}, Name = {4}, Price = {5}, PricePerDay = {6} WHERE Id = {7}",
                        gokart.Description, gokart.EngineSize, gokart.ImageUrl, gokart.Manufacturer, gokart.Name, gokart.Price, gokart.PricePerDay, gokart.Id
                    );
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GokartExists(gokart.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while updating a Gokart.");
                    ModelState.AddModelError(string.Empty, "An error occurred while saving. Please try again.");
                    return RedirectToAction(nameof(Index));
                }
{
    // Re-enable the trigger
    await _context.Database.ExecuteSqlRawAsync("ENABLE TRIGGER GokartPricePerDayChange ON Gokart");
}
            }
            return View(gokart);
        }

        // GET: Gokarts/Delete/5
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gokart = await _context.Gokart
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gokart == null)
            {
                return NotFound();
            }

            return View(gokart);
        }

        // POST: Gokarts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gokart = await _context.Gokart.FindAsync(id);
            if (gokart != null)
            {
                _context.Gokart.Remove(gokart);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GokartExists(int id)
        {
            return _context.Gokart.Any(e => e.Id == id);
        }
    }
}
