// Models/Insurance.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CargoTransportation.Models
{
    [Table("Insurances")]
    public class Insurance
    {
        [Key]
        public int InsuranceID { get; set; }

        public int OrderID { get; set; }

        [MaxLength(100)]
        public string InsuranceCompany { get; set; }

        [MaxLength(50)]
        public string PolicyNumber { get; set; }

        public decimal CoverageAmount { get; set; }
        public decimal Premium { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }

        [ForeignKey("OrderID")]
        public virtual Order Order { get; set; }
    }
}