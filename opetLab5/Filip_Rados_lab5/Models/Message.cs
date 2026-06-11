using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Filip_Rados_lab5.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }

        public string Content { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsRead { get; set; }

        [ForeignKey(nameof(Sender))]
        public int SenderId { get; set; }
        public virtual User Sender { get; set; }

        [ForeignKey(nameof(Receiver))]
        public int ReceiverId { get; set; }
        public virtual User Receiver { get; set; }
    }
}
