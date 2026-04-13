using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CargoTransportation.Models
{
    [Table("Clients")]
    public class Client
    {
        [Key]
        public int ClientID { get; set; }

        public int UserID { get; set; }

        [MaxLength(100)]
        public string CompanyName { get; set; }

        [MaxLength(12)]
        public string INN { get; set; }

        [MaxLength(200)]
        public string LegalAddress { get; set; }

        [MaxLength(100)]
        public string ContactPerson { get; set; }

        [MaxLength(20)]
        public string ContactPhone { get; set; }

        [ForeignKey("UserID")]
        public virtual User User { get; set; }
    }
}