// Models/Payment.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CargoTransportation.Models
{
    [Table("Payments")]
    public class Payment
    {
        [Key]
        public int PaymentID { get; set; }

        public int OrderID { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }

        [MaxLength(50)]
        public string PaymentMethod { get; set; }

        [MaxLength(100)]
        public string TransactionID { get; set; }

        [MaxLength(50)]
        public string Status { get; set; }

        [ForeignKey("OrderID")]
        public virtual Order Order { get; set; }

        public Payment()
        {
            PaymentDate = DateTime.Now;
            Status = "Pending";
        }
    }
}