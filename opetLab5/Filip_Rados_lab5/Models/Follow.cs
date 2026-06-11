using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Filip_Rados_lab5.Models
{
    public class Follow
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Follower))]
        public int FollowerId { get; set; }
        public virtual User Follower { get; set; }

        [ForeignKey(nameof(Following))]
        public int FollowingId { get; set; }
        public virtual User Following { get; set; }
    }
}
