using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CargoTransportation.Models
{
    [Table("Orders")]
    public class Order
    {
        [Key]
        public int OrderID { get; set; }

        [Required]
        [MaxLength(50)]
        public string OrderNumber { get; set; }

        public int ClientID { get; set; }
        public int DriverID { get; set; }
        public int VehicleID { get; set; }
        public int CargoID { get; set; }
        public int StatusID { get; set; }

        [Required]
        [MaxLength(200)]
        public string PickupAddress { get; set; }

        [Required]
        [MaxLength(200)]
        public string DeliveryAddress { get; set; }

        public DateTime? PickupDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public DateTime OrderDate { get; set; }

        public decimal Price { get; set; }
        public decimal Distance { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [ForeignKey("ClientID")]
        public virtual Client Client { get; set; }

        [ForeignKey("DriverID")]
        public virtual Driver Driver { get; set; }

        [ForeignKey("VehicleID")]
        public virtual Vehicle Vehicle { get; set; }

        [ForeignKey("CargoID")]
        public virtual Cargo Cargo { get; set; }

        [ForeignKey("StatusID")]
        public virtual OrderStatus Status { get; set; }

        public Order()
        {
            OrderDate = DateTime.Now;
        }
    }
}