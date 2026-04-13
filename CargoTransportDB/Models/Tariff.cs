// Models/Tariff.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CargoTransportation.Models
{
    [Table("Tariffs")]
    public class Tariff
    {
        [Key]
        public int TariffID { get; set; }

        [Required]
        [MaxLength(50)]
        public string TariffName { get; set; }

        public decimal PricePerKm { get; set; }
        public decimal PricePerKg { get; set; }
        public decimal MinPrice { get; set; }
    }
}