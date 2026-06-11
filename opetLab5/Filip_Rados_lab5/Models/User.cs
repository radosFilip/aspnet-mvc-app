using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Filip_Rados_lab5.Models
{
    public class User : IdentityUser<int>
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string? OIB { get; set; }
        public string? JMBG { get; set; }

        [NotMapped]
        public string Username
        {
            get => UserName ?? string.Empty;
            set => UserName = value;
        }

        [InverseProperty(nameof(Post.Author))]
        public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

        [InverseProperty(nameof(Comment.Author))]
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

        [InverseProperty(nameof(Like.User))]
        public virtual ICollection<Like> Likes { get; set; } = new List<Like>();

        [InverseProperty(nameof(Message.Sender))]
        public virtual ICollection<Message> SentMessages { get; set; } = new List<Message>();

        [InverseProperty(nameof(Message.Receiver))]
        public virtual ICollection<Message> ReceivedMessages { get; set; } = new List<Message>();

        [InverseProperty(nameof(Notification.Recipient))]
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

        [InverseProperty(nameof(Report.Reporter))]
        public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

        [InverseProperty(nameof(Follow.Follower))]
        public virtual ICollection<Follow> Following { get; set; } = new List<Follow>();

        [InverseProperty(nameof(Follow.Following))]
        public virtual ICollection<Follow> Followers { get; set; } = new List<Follow>();

        [InverseProperty(nameof(UserProfileImage.User))]
        public virtual ICollection<UserProfileImage> ProfileImages { get; set; } = new List<UserProfileImage>();
    }
}
