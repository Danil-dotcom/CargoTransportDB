using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CargoTransportation.Models
{
    [Table("Drivers")]
    public class Driver
    {
        [Key]
        public int DriverID { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }

        [Required]
        [MaxLength(20)]
        public string LicenseNumber { get; set; }

        [MaxLength(20)]
        public string Phone { get; set; }

        public DateTime HireDate { get; set; } = DateTime.Now;
        public decimal Salary { get; set; } = 0;

        [MaxLength(50)]
        public string Status { get; set; } = "Available";
    }
}