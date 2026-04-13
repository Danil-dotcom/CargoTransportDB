// Models/Notification.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CargoTransportation.Models
{
    [Table("Notifications")]
    public class Notification
    {
        [Key]
        public int NotificationID { get; set; }

        public int UserID { get; set; }

        [Required]
        [MaxLength(500)]
        public string Message { get; set; }

        public bool IsRead { get; set; }
        public DateTime CreatedDate { get; set; }

        [MaxLength(50)]
        public string Type { get; set; }

        [ForeignKey("UserID")]
        public virtual User User { get; set; }

        public Notification()
        {
            IsRead = false;
            CreatedDate = DateTime.Now;
        }
    }
}