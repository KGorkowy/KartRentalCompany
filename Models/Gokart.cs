using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace KartRentalCompany.Models
{
    public class Gokart
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Manufacturer { get; set; }
        public int Year { get; set; }
        public string? Image { get; set; }
        public decimal Price { get; set; }
        public decimal PricePerDay { get; set; }
    }
}
