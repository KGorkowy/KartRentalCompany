using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KartRentalCompany.Data;
using KartRentalCompany.Models;
using Microsoft.AspNetCore.Authorization;

namespace KartRentalCompany.Controllers
{
    [Authorize]
    public class GokartsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GokartsController(ApplicationDbContext context)
        {
            _context = context;
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
        public async Task<IActionResult> Create([Bind("Id,Name,Manufacturer,Price,PricePerDay,EngineSize,Description,ImageUrl")] Gokart gokart)
        {
            if (ModelState.IsValid)
            {
                _context.Add(gokart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
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
                    _context.Update(gokart);
                    await _context.SaveChangesAsync();
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
                return RedirectToAction(nameof(Index));
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
