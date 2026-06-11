using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Filip_Rados_lab5.Models
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
        [Key]
        public int Id { get; set; }

        public string Title { get; set; }
        public string Content { get; set; }
        public PostCategory Category { get; set; }
        public DateTime CreatedAt { get; set; }

        [ForeignKey(nameof(Author))]
        public int AuthorId { get; set; }
        public virtual User Author { get; set; }

        [InverseProperty(nameof(Comment.Post))]
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

        [InverseProperty(nameof(Like.Post))]
        public virtual ICollection<Like> Likes { get; set; } = new List<Like>();

        [InverseProperty(nameof(Report.Post))]
        public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

        [InverseProperty(nameof(PostVideo.Post))]
        public virtual ICollection<PostVideo> Videos { get; set; } = new List<PostVideo>();

        public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
    }
}
