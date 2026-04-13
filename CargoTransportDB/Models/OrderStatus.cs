// Models/OrderStatus.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CargoTransportation.Models
{
    [Table("OrderStatuses")]
    public class OrderStatus
    {
        [Key]
        public int StatusID { get; set; }

        [Required]
        [MaxLength(50)]
        public string StatusName { get; set; }
    }
}