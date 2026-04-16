namespace Filip_Rados_lab2.Models
{
    public class Follow
    {
        public int Id { get; set; }
        public int FollowerId { get; set; }
        public User Follower { get; set; }
        public int FollowingId { get; set; }
        public User Following { get; set; }
    }
}
