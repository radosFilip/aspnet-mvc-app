namespace Filip_Rados_lab2.Models
{
    public enum ReportReason
    {
        Spam,
        Harassment,
        Inappropriate,
        FakeNews,
        Other
    }

    public enum ReportStatus
    {
        Pending,
        Reviewed,
        Dismissed
    }
    public class Report
    {
        public int Id { get; set; }
        public ReportReason Reason { get; set; }
        public ReportStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ReporterId { get; set; }
        public User Reporter { get; set; }
        public int PostId { get; set; }
        public Post Post { get; set; }
    }
}
