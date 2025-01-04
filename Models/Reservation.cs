using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KartRentalCompany.Models
{
    public class Reservation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public IdentityUser User { get; set; } // Navigation property

        [Required]
        public int GokartId { get; set; }
        [ForeignKey("GokartId")]
        public Gokart Gokart { get; set; } // Navigation property

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public bool IsApproved { get; set; }
        public bool IsTakenAway { get; set; }
        public bool IsReturned { get; set; }
        public bool IsCancelled { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalCost { get; set; }
    }
}
