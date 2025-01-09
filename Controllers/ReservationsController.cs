using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KartRentalCompany.Data;
using KartRentalCompany.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;

namespace KartRentalCompany.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<ReservationsController> _logger;

        public ReservationsController(ApplicationDbContext context, UserManager<IdentityUser> userManager, ILogger<ReservationsController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // GET: Reservations
        public async Task<IActionResult> Index()
        {
            IQueryable<Reservation> reservations;

            if (User.IsInRole("Admin"))
            {
                reservations = _context.Reservation.Include(r => r.Gokart).Include(r => r.User);
            }
            else
            {
                var userEmail = User.Identity.Name;
                reservations = _context.Reservation.Include(r => r.Gokart).Include(r => r.User).Where(r => r.User.Email == userEmail);
            }

            return View(await reservations.ToListAsync());
        }

        // GET: Reservations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservation
                .Include(r => r.Gokart)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // GET: Reservations/Create
        public IActionResult Create()
        {
            if (User.IsInRole("Admin"))
            {
                ViewBag.Users = new SelectList(_context.Users, "Email", "Email");
            }
            ViewBag.Gokarts = new SelectList(_context.Gokart, "Id", "Name");
            return View();
        }

        // POST: Reservations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string userEmail, int gokartId, DateTime startDate, DateTime endDate)
        {
            _logger.LogInformation("Received userEmail: {UserEmail}", userEmail);

            string userId;
            if (User.IsInRole("Admin"))
            {
                if (string.IsNullOrEmpty(userEmail))
                {
                    ModelState.AddModelError(string.Empty, "User email is required for admin.");
                    ViewBag.Users = new SelectList(_context.Users, "Email", "Email");
                    ViewBag.Gokarts = new SelectList(_context.Gokart, "Id", "Name");
                    return View();
                }

                var user = await _userManager.FindByEmailAsync(userEmail);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "The specified user does not exist.");
                    ViewBag.Users = new SelectList(_context.Users, "Email", "Email");
                    ViewBag.Gokarts = new SelectList(_context.Gokart, "Id", "Name");
                    return View();
                }
                userId = user.Id;
            }
            else
            {
                userId = _userManager.GetUserId(User) ?? throw new InvalidOperationException("User ID cannot be null.");
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    throw new InvalidOperationException("User not found.");
                }
                userEmail = user.Email;
            }

            _logger.LogInformation("Using userId: {UserId} and userEmail: {UserEmail}", userId, userEmail);

            if (startDate.Date > endDate.Date)
            {
                ModelState.AddModelError(string.Empty, "The start date must be before or the same as the end date.");
            }

            if (startDate.Date < DateTime.Now.Date)
            {
                ModelState.AddModelError(string.Empty, "The start date must be today or in the future.");
            }

            if (!ModelState.IsValid)
            {
                if (User.IsInRole("Admin"))
                {
                    ViewBag.Users = new SelectList(_context.Users, "Email", "Email");
                }
                ViewBag.Gokarts = new SelectList(_context.Gokart, "Id", "Name");
                return View();
            }

            var isAvailable = await IsGokartAvailable(gokartId, startDate, endDate);
            if (!isAvailable)
            {
                ModelState.AddModelError(string.Empty, "The gokart is not available for the selected time period.");
                if (User.IsInRole("Admin"))
                {
                    ViewBag.Users = new SelectList(_context.Users, "Email", "Email");
                }
                ViewBag.Gokarts = new SelectList(_context.Gokart, "Id", "Name");
                return View();
            }

            try
            {
                var reservation = new Reservation
                {
                    UserId = userId,
                    GokartId = gokartId,
                    StartDate = startDate,
                    EndDate = endDate,
                    TotalCost = await CalculateTotalCost(gokartId, startDate, endDate)
                };

                _context.Add(reservation);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Reservation created successfully with ID: {ReservationId}", reservation.Id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the reservation.");
                ModelState.AddModelError(string.Empty, "An error occurred while saving the reservation. Please try again.");
                if (User.IsInRole("Admin"))
                {
                    ViewBag.Users = new SelectList(_context.Users, "Email", "Email");
                }
                ViewBag.Gokarts = new SelectList(_context.Gokart, "Id", "Name");
                return View();
            }
        }


        // GET: Reservations/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservation
                .Include(r => r.Gokart)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            ViewBag.IsApproved = reservation.IsApproved;
            ViewBag.IsTakenAway = reservation.IsTakenAway;
            ViewBag.IsReturned = reservation.IsReturned;
            ViewBag.IsCancelled = reservation.IsCancelled;

            return View(reservation);
        }


        // POST: Reservations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, bool isApproved, bool isTakenAway, bool isReturned, bool isCancelled)
        {
            var reservation = await _context.Reservation.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }

            reservation.IsApproved = isApproved;
            reservation.IsTakenAway = isTakenAway;
            reservation.IsReturned = isReturned;
            reservation.IsCancelled = isCancelled;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reservation);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Reservation updated successfully with ID: {ReservationId}", reservation.Id);
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservationExists(reservation.Id))
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
                    _logger.LogError(ex, "Error occurred while updating reservation ID: {ReservationId}", reservation.Id);
                    ModelState.AddModelError(string.Empty, "An error occurred while saving the reservation. Please try again.");
                }
            }
            else
            {
                _logger.LogWarning("Model state is invalid for reservation ID: {ReservationId}. Errors: {Errors}",
                    reservation.Id, ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            }

            return View(reservation);
        }

        private bool ReservationExists(int id)
        {
            return _context.Reservation.Any(e => e.Id == id);
        }

        // GET: Reservations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservation
                .Include(r => r.Gokart)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reservation = await _context.Reservation.FindAsync(id);
            if (reservation != null)
            {
                _context.Reservation.Remove(reservation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var reservation = await _context.Reservation.Include(r => r.User).FirstOrDefaultAsync(r => r.Id == id);
            if (reservation == null || reservation.User.Email != User.Identity.Name)
            {
                return NotFound();
            }

            reservation.IsCancelled = true;
            _context.Update(reservation);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Reservation with ID {ReservationId} has been cancelled by the user.", id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> IsGokartAvailable(int gokartId, DateTime startDate, DateTime endDate, int? reservationId = null)
        {
            var conflictingReservations = await _context.Reservation
                .Where(r => r.GokartId == gokartId && r.Id != reservationId && r.StartDate < endDate && r.EndDate >= startDate)
                .ToListAsync();

            return !conflictingReservations.Any();
        }

        private async Task<decimal> CalculateTotalCost(int gokartId, DateTime startDate, DateTime endDate)
        {
            var gokart = await _context.Gokart.FindAsync(gokartId);
            if (gokart == null)
            {
                throw new Exception("Gokart not found");
            }

            var days = (endDate - startDate).Days + 1;
            var totalCost = await _context.Reservation
                .FromSqlRaw("SELECT dbo.CalculateRentalCost({0}, {1}) AS TotalCost", gokart.PricePerDay, days)
                .Select(r => r.TotalCost)
                .FirstOrDefaultAsync();

            return totalCost;
        }

    }
}

/* ToDo: allow admins to manage bools in edit view
 fix date displaying
 add cost formatting 
 index shouldnt display reservations if IsReturned is true */