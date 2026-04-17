using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CargoTransportation.Models
{
    [Table("Vehicles")]
    public class Vehicle
    {
        [Key]
        public int VehicleID { get; set; }

        [Required]
        [MaxLength(15)]
        public string PlateNumber { get; set; }

        [MaxLength(50)]
        public string Brand { get; set; }

        [MaxLength(50)]
        public string Model { get; set; }

        public int Year { get; set; }
        public decimal Capacity { get; set; }
        public decimal LoadCapacity { get; set; }

        [MaxLength(50)]
        public string Status { get; set; }

        [MaxLength(500)]
        public string ImagePath { get; set; }

        public Vehicle()
        {
            Status = "Available";
        }
    }
}