using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Filip_Rados_lab5.Models
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
        [Key]
        public int Id { get; set; }

        public ReportReason Reason { get; set; }
        public ReportStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }

        [ForeignKey(nameof(Reporter))]
        public int ReporterId { get; set; }
        public virtual User Reporter { get; set; }

        [ForeignKey(nameof(Post))]
        public int PostId { get; set; }
        public virtual Post Post { get; set; }
    }
}
