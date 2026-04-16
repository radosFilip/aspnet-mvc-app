namespace Filip_Rados_lab2.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public int RecipientId { get; set; }
        public User Recipient { get; set; }
    }
}
