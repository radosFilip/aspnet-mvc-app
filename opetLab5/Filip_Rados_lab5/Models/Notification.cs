using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Filip_Rados_lab5.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }

        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }

        [ForeignKey(nameof(Recipient))]
        public int RecipientId { get; set; }
        public virtual User Recipient { get; set; }
    }
}
