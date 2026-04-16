namespace Filip_Rados_lab2.Models
{
    public enum PostCategory
    {
        Health,
        Productivity,
        Finance,
        Cooking,
        Technology,
        Home,
        Travel,
        Other
    }
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public PostCategory Category { get; set; }
        public DateTime CreatedAt { get; set; }
        public int AuthorId { get; set; }
        public User Author { get; set; }
        public List<Comment> Comments { get; set; } = new List<Comment>();
        public List<Tag> Tags { get; set; } = new List<Tag>();
    }
}
