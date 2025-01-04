using KartRentalCompany.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using KartRentalCompany.Models;

namespace KartRentalCompany.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<KartRentalCompany.Models.Gokart> Gokart { get; set; } = default!;
        public DbSet<KartRentalCompany.Models.Reservation> Reservation { get; set; } = default!;
    }
}
