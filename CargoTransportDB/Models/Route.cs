// Models/Route.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CargoTransportation.Models
{
    [Table("Routes")]
    public class Route
    {
        [Key]
        public int RouteID { get; set; }

        public int OrderID { get; set; }
        public string Waypoints { get; set; }
        public DateTime? ActualStartTime { get; set; }
        public DateTime? ActualEndTime { get; set; }
        public decimal FuelConsumption { get; set; }

        [MaxLength(500)]
        public string Notes { get; set; }

        [ForeignKey("OrderID")]
        public virtual Order Order { get; set; }
    }
}