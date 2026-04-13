// Models/Review.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CargoTransportation.Models
{
    [Table("Reviews")]
    public class Review
    {
        [Key]
        public int ReviewID { get; set; }

        public int OrderID { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(500)]
        public string Comment { get; set; }

        public DateTime ReviewDate { get; set; }

        [ForeignKey("OrderID")]
        public virtual Order Order { get; set; }

        public Review()
        {
            ReviewDate = DateTime.Now;
        }
    }
}