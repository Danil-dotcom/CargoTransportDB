// Models/CargoType.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CargoTransportation.Models
{
    [Table("CargoTypes")]
    public class CargoType
    {
        [Key]
        public int CargoTypeID { get; set; }

        [Required]
        [MaxLength(50)]
        public string TypeName { get; set; }

        public bool RequiresSpecialCondition { get; set; }
    }
}