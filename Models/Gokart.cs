using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KartRentalCompany.Models
{
    public class Gokart
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Manufacturer { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }
        [Display(Name = "Price Per Day")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal PricePerDay { get; set; }
        [Display(Name = "Engine Size")]
        public int EngineSize { get; set; }
        public string Description { get; set; }
        [Display(Name = "Image")]
        public string ImageUrl { get; set; }
    }
}
