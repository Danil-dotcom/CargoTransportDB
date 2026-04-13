// Models/Cargo.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CargoTransportation.Models
{
    [Table("Cargos")]
    public class Cargo
    {
        [Key]
        public int CargoID { get; set; }

        public int CargoTypeID { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public decimal Weight { get; set; }
        public decimal Volume { get; set; }
        public bool DangerousGoods { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [ForeignKey("CargoTypeID")]
        public virtual CargoType CargoType { get; set; }
    }
}