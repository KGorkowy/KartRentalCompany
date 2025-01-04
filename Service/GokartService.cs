using KartRentalCompany.Data;
using KartRentalCompany.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace KartRentalCompany.Services
{
    public class GokartService
    {
        private readonly ApplicationDbContext _context;

        public GokartService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Gokart>> GetGokartsAsync()
        {
            return await _context.Gokarts.ToListAsync();
        }

        public async Task<Gokart?> GetGokartByIdAsync(int id)
        {
            return await _context.Gokarts.FindAsync(id);
        }

        public async Task AddGokartAsync(Gokart gokart)
        {
            _context.Gokarts.Add(gokart);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateGokartAsync(Gokart gokart)
        {
            _context.Gokarts.Update(gokart);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveGokartAsync(int id)
        {
            var gokart = await _context.Gokarts.FindAsync(id);
            if (gokart != null)
            {
                _context.Gokarts.Remove(gokart);
                await _context.SaveChangesAsync();
            }
        }
    }
}