using Filip_Rados_lab5.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Filip_Rados_lab5.Data
{
    public class AppDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public override DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Follow> Follows { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<PostVideo> PostVideos { get; set; }
        public DbSet<UserProfileImage> UserProfileImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<User>()
                .Property(user => user.UserName)
                .HasColumnName("Username");
            modelBuilder.Entity<IdentityRole<int>>().ToTable("Roles");
            modelBuilder.Entity<IdentityUserRole<int>>().ToTable("UserRoles");
            modelBuilder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims");
            modelBuilder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins");
            modelBuilder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims");
            modelBuilder.Entity<IdentityUserToken<int>>().ToTable("UserTokens");

            modelBuilder.Entity<Post>()
                .HasOne(post => post.Author)
                .WithMany(user => user.Posts)
                .HasForeignKey(post => post.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Comment>()
                .HasOne(comment => comment.Author)
                .WithMany(user => user.Comments)
                .HasForeignKey(comment => comment.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Comment>()
                .HasOne(comment => comment.Post)
                .WithMany(post => post.Comments)
                .HasForeignKey(comment => comment.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Post>()
                .HasMany(post => post.Tags)
                .WithMany(tag => tag.Posts)
                .UsingEntity(join => join.ToTable("PostTags"));

            modelBuilder.Entity<Like>()
                .HasOne(like => like.User)
                .WithMany(user => user.Likes)
                .HasForeignKey(like => like.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Like>()
                .HasOne(like => like.Post)
                .WithMany(post => post.Likes)
                .HasForeignKey(like => like.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Like>()
                .HasIndex(like => new { like.UserId, like.PostId })
                .IsUnique();

            modelBuilder.Entity<Follow>()
                .HasOne(follow => follow.Follower)
                .WithMany(user => user.Following)
                .HasForeignKey(follow => follow.FollowerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Follow>()
                .HasOne(follow => follow.Following)
                .WithMany(user => user.Followers)
                .HasForeignKey(follow => follow.FollowingId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Follow>()
                .HasIndex(follow => new { follow.FollowerId, follow.FollowingId })
                .IsUnique();

            modelBuilder.Entity<Message>()
                .HasOne(message => message.Sender)
                .WithMany(user => user.SentMessages)
                .HasForeignKey(message => message.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(message => message.Receiver)
                .WithMany(user => user.ReceivedMessages)
                .HasForeignKey(message => message.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notification>()
                .HasOne(notification => notification.Recipient)
                .WithMany(user => user.Notifications)
                .HasForeignKey(notification => notification.RecipientId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Report>()
                .HasOne(report => report.Reporter)
                .WithMany(user => user.Reports)
                .HasForeignKey(report => report.ReporterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Report>()
                .HasOne(report => report.Post)
                .WithMany(post => post.Reports)
                .HasForeignKey(report => report.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PostVideo>()
                .HasOne(video => video.Post)
                .WithMany(post => post.Videos)
                .HasForeignKey(video => video.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserProfileImage>()
                .HasOne(image => image.User)
                .WithMany(user => user.ProfileImages)
                .HasForeignKey(image => image.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
