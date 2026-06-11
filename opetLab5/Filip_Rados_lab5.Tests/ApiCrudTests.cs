using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Filip_Rados_lab5.Data;
using Filip_Rados_lab5.Dtos;
using Filip_Rados_lab5.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Filip_Rados_lab5.Tests;

public class ApiCrudTests
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    [Fact]
    public async Task UsersApi_ShouldCoverCrud_NotFound_AndValidation()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = factory.CreateClient();

        var existingUserId = await WithDb(factory, db => CreateUserAsync(db, "ana"));

        var listResponse = await client.GetAsync("/api/users");
        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);
        Assert.NotEmpty(await ReadJsonAsync<List<UserDto>>(listResponse));

        var getResponse = await client.GetAsync($"/api/users/{existingUserId}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.Equal(existingUserId, (await ReadJsonAsync<UserDto>(getResponse)).Id);

        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("/api/users/999999")).StatusCode);

        var createResponse = await client.PostAsJsonAsync("/api/users", new UserCreateUpdateDto
        {
            FirstName = "Ivan",
            LastName = "Ivic",
            Username = "ivan",
            Email = "ivan@example.com",
            DateOfBirth = new DateTime(1998, 5, 10)
        });
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await ReadJsonAsync<UserDto>(createResponse);
        Assert.Equal("ivan", created.Username);

        var invalidResponse = await client.PostAsJsonAsync("/api/users", new UserCreateUpdateDto
        {
            FirstName = "I",
            LastName = "Ivic",
            Username = "ivan",
            Email = "not-an-email",
            DateOfBirth = null
        });
        Assert.Equal(HttpStatusCode.BadRequest, invalidResponse.StatusCode);

        var updateResponse = await client.PutAsJsonAsync($"/api/users/{created.Id}", new UserCreateUpdateDto
        {
            FirstName = "Marko",
            LastName = "Markovic",
            Username = "marko",
            Email = "marko@example.com",
            DateOfBirth = new DateTime(1997, 1, 1)
        });
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        Assert.Equal("marko", (await ReadJsonAsync<UserDto>(updateResponse)).Username);

        var missingPut = await client.PutAsJsonAsync("/api/users/999999", ValidUser("missing", "missing@example.com"));
        Assert.Equal(HttpStatusCode.NotFound, missingPut.StatusCode);

        var deleteResponse = await client.DeleteAsync($"/api/users/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        Assert.False(await WithDb(factory, db => db.Users.AnyAsync(user => user.Id == created.Id)));

        Assert.Equal(HttpStatusCode.NotFound, (await client.DeleteAsync("/api/users/999999")).StatusCode);
    }

    [Fact]
    public async Task TagsApi_ShouldCoverCrud_NotFound_AndValidation()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = factory.CreateClient();

        var existingTagId = await WithDb(factory, db => CreateTagAsync(db, "Health"));

        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/api/tags")).StatusCode);
        var getResponse = await client.GetAsync($"/api/tags/{existingTagId}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.Equal("Health", (await ReadJsonAsync<TagDto>(getResponse)).Name);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("/api/tags/999999")).StatusCode);

        var createResponse = await client.PostAsJsonAsync("/api/tags", new TagCreateUpdateDto { Name = "Productivity" });
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await ReadJsonAsync<TagDto>(createResponse);

        var invalidResponse = await client.PostAsJsonAsync("/api/tags", new TagCreateUpdateDto { Name = "x" });
        Assert.Equal(HttpStatusCode.BadRequest, invalidResponse.StatusCode);

        var updateResponse = await client.PutAsJsonAsync($"/api/tags/{created.Id}", new TagCreateUpdateDto { Name = "Cooking" });
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        Assert.Equal("Cooking", (await ReadJsonAsync<TagDto>(updateResponse)).Name);

        var missingPut = await client.PutAsJsonAsync("/api/tags/999999", new TagCreateUpdateDto { Name = "Other" });
        Assert.Equal(HttpStatusCode.NotFound, missingPut.StatusCode);

        var deleteResponse = await client.DeleteAsync($"/api/tags/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        Assert.False(await WithDb(factory, db => db.Tags.AnyAsync(tag => tag.Id == created.Id)));
        Assert.Equal(HttpStatusCode.NotFound, (await client.DeleteAsync("/api/tags/999999")).StatusCode);
    }

    [Fact]
    public async Task PostsApi_ShouldCoverCrud_NotFound_AndValidation()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = factory.CreateClient();

        var seed = await WithDb(factory, async db =>
        {
            var userId = await CreateUserAsync(db, "post-author");
            var tagId = await CreateTagAsync(db, "Tips");
            var postId = await CreatePostAsync(db, userId);
            return new { userId, tagId, postId };
        });

        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/api/posts")).StatusCode);
        var getResponse = await client.GetAsync($"/api/posts/{seed.postId}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.Equal(seed.postId, (await ReadJsonAsync<PostDto>(getResponse)).Id);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("/api/posts/999999")).StatusCode);

        var createResponse = await client.PostAsJsonAsync("/api/posts", new PostCreateUpdateDto
        {
            Title = "Created post title",
            Content = "Created post content for integration testing.",
            Category = PostCategory.Productivity,
            AuthorId = seed.userId,
            TagIds = new List<int> { seed.tagId }
        });
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await ReadJsonAsync<PostDto>(createResponse);
        Assert.Equal("Created post title", created.Title);

        var invalidResponse = await client.PostAsJsonAsync("/api/posts", new PostCreateUpdateDto
        {
            Title = "Bad",
            Content = "Too short",
            Category = PostCategory.Health,
            AuthorId = 999999
        });
        Assert.Equal(HttpStatusCode.BadRequest, invalidResponse.StatusCode);

        var updateResponse = await client.PutAsJsonAsync($"/api/posts/{created.Id}", new PostCreateUpdateDto
        {
            Title = "Updated post title",
            Content = "Updated post content for integration testing.",
            Category = PostCategory.Technology,
            AuthorId = seed.userId
        });
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        Assert.Equal("Updated post title", (await ReadJsonAsync<PostDto>(updateResponse)).Title);

        var missingPut = await client.PutAsJsonAsync("/api/posts/999999", ValidPost(seed.userId));
        Assert.Equal(HttpStatusCode.NotFound, missingPut.StatusCode);

        var deleteResponse = await client.DeleteAsync($"/api/posts/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        Assert.False(await WithDb(factory, db => db.Posts.AnyAsync(post => post.Id == created.Id)));
        Assert.Equal(HttpStatusCode.NotFound, (await client.DeleteAsync("/api/posts/999999")).StatusCode);
    }

    [Fact]
    public async Task CommentsApi_ShouldCoverCrud_NotFound_AndValidation()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = factory.CreateClient();

        var seed = await SeedPostGraph(factory);
        var commentId = await WithDb(factory, async db =>
        {
            var comment = new Comment
            {
                Content = "Seed comment",
                CreatedAt = DateTime.UtcNow,
                AuthorId = seed.UserId,
                PostId = seed.PostId
            };
            db.Comments.Add(comment);
            await db.SaveChangesAsync();
            return comment.Id;
        });

        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/api/comments")).StatusCode);
        var getResponse = await client.GetAsync($"/api/comments/{commentId}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.Equal(commentId, (await ReadJsonAsync<CommentDto>(getResponse)).Id);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("/api/comments/999999")).StatusCode);

        var createResponse = await client.PostAsJsonAsync("/api/comments", new CommentCreateUpdateDto
        {
            Content = "Created comment",
            AuthorId = seed.UserId,
            PostId = seed.PostId
        });
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await ReadJsonAsync<CommentDto>(createResponse);

        var invalidResponse = await client.PostAsJsonAsync("/api/comments", new CommentCreateUpdateDto
        {
            Content = "x",
            AuthorId = seed.UserId,
            PostId = 999999
        });
        Assert.Equal(HttpStatusCode.BadRequest, invalidResponse.StatusCode);

        var updateResponse = await client.PutAsJsonAsync($"/api/comments/{created.Id}", new CommentCreateUpdateDto
        {
            Content = "Updated comment",
            AuthorId = seed.UserId,
            PostId = seed.PostId
        });
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        Assert.Equal("Updated comment", (await ReadJsonAsync<CommentDto>(updateResponse)).Content);

        var missingPut = await client.PutAsJsonAsync("/api/comments/999999", ValidComment(seed.UserId, seed.PostId));
        Assert.Equal(HttpStatusCode.NotFound, missingPut.StatusCode);

        var deleteResponse = await client.DeleteAsync($"/api/comments/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        Assert.False(await WithDb(factory, db => db.Comments.AnyAsync(comment => comment.Id == created.Id)));
        Assert.Equal(HttpStatusCode.NotFound, (await client.DeleteAsync("/api/comments/999999")).StatusCode);
    }

    [Fact]
    public async Task MessagesApi_ShouldCoverCrud_NotFound_AndValidation()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = factory.CreateClient();

        var users = await WithDb(factory, async db =>
        {
            var senderId = await CreateUserAsync(db, "sender");
            var receiverId = await CreateUserAsync(db, "receiver");
            var thirdId = await CreateUserAsync(db, "third");
            return new { senderId, receiverId, thirdId };
        });
        var messageId = await WithDb(factory, async db =>
        {
            var message = new Message
            {
                Content = "Seed message",
                SentAt = DateTime.UtcNow,
                SenderId = users.senderId,
                ReceiverId = users.receiverId
            };
            db.Messages.Add(message);
            await db.SaveChangesAsync();
            return message.Id;
        });

        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/api/messages")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync($"/api/messages/{messageId}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("/api/messages/999999")).StatusCode);

        var createResponse = await client.PostAsJsonAsync("/api/messages", new MessageCreateUpdateDto
        {
            Content = "Created message",
            SenderId = users.senderId,
            ReceiverId = users.receiverId
        });
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await ReadJsonAsync<MessageDto>(createResponse);

        var invalidResponse = await client.PostAsJsonAsync("/api/messages", new MessageCreateUpdateDto
        {
            Content = "x",
            SenderId = users.senderId,
            ReceiverId = users.senderId
        });
        Assert.Equal(HttpStatusCode.BadRequest, invalidResponse.StatusCode);

        var updateResponse = await client.PutAsJsonAsync($"/api/messages/{created.Id}", new MessageCreateUpdateDto
        {
            Content = "Updated message",
            SenderId = users.receiverId,
            ReceiverId = users.thirdId,
            IsRead = true
        });
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        Assert.True((await ReadJsonAsync<MessageDto>(updateResponse)).IsRead);

        var missingPut = await client.PutAsJsonAsync("/api/messages/999999", ValidMessage(users.senderId, users.receiverId));
        Assert.Equal(HttpStatusCode.NotFound, missingPut.StatusCode);

        var deleteResponse = await client.DeleteAsync($"/api/messages/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        Assert.False(await WithDb(factory, db => db.Messages.AnyAsync(message => message.Id == created.Id)));
        Assert.Equal(HttpStatusCode.NotFound, (await client.DeleteAsync("/api/messages/999999")).StatusCode);
    }

    [Fact]
    public async Task ReportsApi_ShouldCoverCrud_NotFound_AndValidation()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = factory.CreateClient();

        var seed = await SeedPostGraph(factory);
        var reportId = await WithDb(factory, async db =>
        {
            var report = new Report
            {
                Reason = ReportReason.Spam,
                Status = ReportStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                ReporterId = seed.UserId,
                PostId = seed.PostId
            };
            db.Reports.Add(report);
            await db.SaveChangesAsync();
            return report.Id;
        });

        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/api/reports")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync($"/api/reports/{reportId}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("/api/reports/999999")).StatusCode);

        var createResponse = await client.PostAsJsonAsync("/api/reports", new ReportCreateUpdateDto
        {
            Reason = ReportReason.Inappropriate,
            Status = ReportStatus.Pending,
            ReporterId = seed.UserId,
            PostId = seed.PostId
        });
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await ReadJsonAsync<ReportDto>(createResponse);

        var invalidResponse = await client.PostAsJsonAsync("/api/reports", new ReportCreateUpdateDto
        {
            Reason = ReportReason.Spam,
            Status = ReportStatus.Pending,
            ReporterId = 999999,
            PostId = seed.PostId
        });
        Assert.Equal(HttpStatusCode.BadRequest, invalidResponse.StatusCode);

        var updateResponse = await client.PutAsJsonAsync($"/api/reports/{created.Id}", new ReportCreateUpdateDto
        {
            Reason = ReportReason.Other,
            Status = ReportStatus.Reviewed,
            ReporterId = seed.UserId,
            PostId = seed.PostId
        });
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        Assert.Equal(ReportStatus.Reviewed, (await ReadJsonAsync<ReportDto>(updateResponse)).Status);

        var missingPut = await client.PutAsJsonAsync("/api/reports/999999", ValidReport(seed.UserId, seed.PostId));
        Assert.Equal(HttpStatusCode.NotFound, missingPut.StatusCode);

        var deleteResponse = await client.DeleteAsync($"/api/reports/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        Assert.False(await WithDb(factory, db => db.Reports.AnyAsync(report => report.Id == created.Id)));
        Assert.Equal(HttpStatusCode.NotFound, (await client.DeleteAsync("/api/reports/999999")).StatusCode);
    }

    [Fact]
    public async Task LikesApi_ShouldCoverCrud_NotFound_AndValidation()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = factory.CreateClient();

        var seed = await WithDb(factory, async db =>
        {
            var firstUserId = await CreateUserAsync(db, "liker-one");
            var secondUserId = await CreateUserAsync(db, "liker-two");
            var firstPostId = await CreatePostAsync(db, firstUserId, "First like target title");
            var secondPostId = await CreatePostAsync(db, secondUserId, "Second like target title");
            return new { firstUserId, secondUserId, firstPostId, secondPostId };
        });
        var likeId = await WithDb(factory, async db =>
        {
            var like = new Like { UserId = seed.firstUserId, PostId = seed.firstPostId };
            db.Likes.Add(like);
            await db.SaveChangesAsync();
            return like.Id;
        });

        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/api/likes")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync($"/api/likes/{likeId}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("/api/likes/999999")).StatusCode);

        var createResponse = await client.PostAsJsonAsync("/api/likes", new LikeCreateUpdateDto
        {
            UserId = seed.secondUserId,
            PostId = seed.firstPostId
        });
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await ReadJsonAsync<LikeDto>(createResponse);

        var invalidResponse = await client.PostAsJsonAsync("/api/likes", new LikeCreateUpdateDto
        {
            UserId = 999999,
            PostId = seed.firstPostId
        });
        Assert.Equal(HttpStatusCode.BadRequest, invalidResponse.StatusCode);

        var updateResponse = await client.PutAsJsonAsync($"/api/likes/{created.Id}", new LikeCreateUpdateDto
        {
            UserId = seed.secondUserId,
            PostId = seed.secondPostId
        });
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        Assert.Equal(seed.secondPostId, (await ReadJsonAsync<LikeDto>(updateResponse)).Post?.Id);

        var missingPut = await client.PutAsJsonAsync("/api/likes/999999", ValidLike(seed.firstUserId, seed.firstPostId));
        Assert.Equal(HttpStatusCode.NotFound, missingPut.StatusCode);

        var deleteResponse = await client.DeleteAsync($"/api/likes/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        Assert.False(await WithDb(factory, db => db.Likes.AnyAsync(like => like.Id == created.Id)));
        Assert.Equal(HttpStatusCode.NotFound, (await client.DeleteAsync("/api/likes/999999")).StatusCode);
    }

    [Fact]
    public async Task FollowsApi_ShouldCoverCrud_NotFound_AndValidation()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = factory.CreateClient();

        var users = await WithDb(factory, async db =>
        {
            var firstId = await CreateUserAsync(db, "follow-one");
            var secondId = await CreateUserAsync(db, "follow-two");
            var thirdId = await CreateUserAsync(db, "follow-three");
            return new { firstId, secondId, thirdId };
        });
        var followId = await WithDb(factory, async db =>
        {
            var follow = new Follow { FollowerId = users.firstId, FollowingId = users.secondId };
            db.Follows.Add(follow);
            await db.SaveChangesAsync();
            return follow.Id;
        });

        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/api/follows")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync($"/api/follows/{followId}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("/api/follows/999999")).StatusCode);

        var createResponse = await client.PostAsJsonAsync("/api/follows", new FollowCreateUpdateDto
        {
            FollowerId = users.secondId,
            FollowingId = users.firstId
        });
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await ReadJsonAsync<FollowDto>(createResponse);

        var invalidResponse = await client.PostAsJsonAsync("/api/follows", new FollowCreateUpdateDto
        {
            FollowerId = users.firstId,
            FollowingId = users.firstId
        });
        Assert.Equal(HttpStatusCode.BadRequest, invalidResponse.StatusCode);

        var updateResponse = await client.PutAsJsonAsync($"/api/follows/{created.Id}", new FollowCreateUpdateDto
        {
            FollowerId = users.thirdId,
            FollowingId = users.firstId
        });
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        Assert.Equal(users.thirdId, (await ReadJsonAsync<FollowDto>(updateResponse)).Follower?.Id);

        var missingPut = await client.PutAsJsonAsync("/api/follows/999999", ValidFollow(users.firstId, users.secondId));
        Assert.Equal(HttpStatusCode.NotFound, missingPut.StatusCode);

        var deleteResponse = await client.DeleteAsync($"/api/follows/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        Assert.False(await WithDb(factory, db => db.Follows.AnyAsync(follow => follow.Id == created.Id)));
        Assert.Equal(HttpStatusCode.NotFound, (await client.DeleteAsync("/api/follows/999999")).StatusCode);
    }

    [Fact]
    public async Task NotificationsApi_ShouldCoverCrud_NotFound_AndValidation()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = factory.CreateClient();

        var userId = await WithDb(factory, db => CreateUserAsync(db, "recipient"));
        var notificationId = await WithDb(factory, async db =>
        {
            var notification = new Notification
            {
                Message = "Seed notification",
                CreatedAt = DateTime.UtcNow,
                RecipientId = userId
            };
            db.Notifications.Add(notification);
            await db.SaveChangesAsync();
            return notification.Id;
        });

        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/api/notifications")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync($"/api/notifications/{notificationId}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("/api/notifications/999999")).StatusCode);

        var createResponse = await client.PostAsJsonAsync("/api/notifications", new NotificationCreateUpdateDto
        {
            Message = "Created notification",
            RecipientId = userId
        });
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await ReadJsonAsync<NotificationDto>(createResponse);

        var invalidResponse = await client.PostAsJsonAsync("/api/notifications", new NotificationCreateUpdateDto
        {
            Message = "x",
            RecipientId = 999999
        });
        Assert.Equal(HttpStatusCode.BadRequest, invalidResponse.StatusCode);

        var updateResponse = await client.PutAsJsonAsync($"/api/notifications/{created.Id}", new NotificationCreateUpdateDto
        {
            Message = "Updated notification",
            RecipientId = userId
        });
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        Assert.Equal("Updated notification", (await ReadJsonAsync<NotificationDto>(updateResponse)).Message);

        var missingPut = await client.PutAsJsonAsync("/api/notifications/999999", ValidNotification(userId));
        Assert.Equal(HttpStatusCode.NotFound, missingPut.StatusCode);

        var deleteResponse = await client.DeleteAsync($"/api/notifications/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        Assert.False(await WithDb(factory, db => db.Notifications.AnyAsync(notification => notification.Id == created.Id)));
        Assert.Equal(HttpStatusCode.NotFound, (await client.DeleteAsync("/api/notifications/999999")).StatusCode);
    }

    private static async Task<T> ReadJsonAsync<T>(HttpResponseMessage response)
    {
        var dto = await response.Content.ReadFromJsonAsync<T>(JsonOptions);
        Assert.NotNull(dto);
        return dto;
    }

    private static async Task<TResult> WithDb<TResult>(
        ApiTestApplicationFactory factory,
        Func<AppDbContext, Task<TResult>> action)
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        return await action(db);
    }

    private static async Task<int> CreateUserAsync(AppDbContext db, string username)
    {
        var user = new User
        {
            FirstName = "Test",
            LastName = "User",
            UserName = username,
            Email = $"{username}@example.com",
            DateOfBirth = new DateTime(1990, 1, 1)
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();
        return user.Id;
    }

    private static async Task<int> CreateTagAsync(AppDbContext db, string name)
    {
        var tag = new Tag { Name = name };
        db.Tags.Add(tag);
        await db.SaveChangesAsync();
        return tag.Id;
    }

    private static async Task<int> CreatePostAsync(
        AppDbContext db,
        int authorId,
        string title = "Seed post title")
    {
        var post = new Post
        {
            Title = title,
            Content = "Seed post content for integration tests.",
            Category = PostCategory.Health,
            CreatedAt = DateTime.UtcNow,
            AuthorId = authorId
        };

        db.Posts.Add(post);
        await db.SaveChangesAsync();
        return post.Id;
    }

    private static async Task<(int UserId, int PostId)> SeedPostGraph(ApiTestApplicationFactory factory)
    {
        return await WithDb(factory, async db =>
        {
            var userId = await CreateUserAsync(db, $"graph-user-{Guid.NewGuid():N}");
            var postId = await CreatePostAsync(db, userId);
            return (userId, postId);
        });
    }

    private static UserCreateUpdateDto ValidUser(string username, string email)
    {
        return new UserCreateUpdateDto
        {
            FirstName = "Valid",
            LastName = "User",
            Username = username,
            Email = email,
            DateOfBirth = new DateTime(1995, 2, 2)
        };
    }

    private static PostCreateUpdateDto ValidPost(int authorId)
    {
        return new PostCreateUpdateDto
        {
            Title = "Valid post title",
            Content = "Valid post content for missing id checks.",
            Category = PostCategory.Other,
            AuthorId = authorId
        };
    }

    private static CommentCreateUpdateDto ValidComment(int authorId, int postId)
    {
        return new CommentCreateUpdateDto
        {
            Content = "Valid comment",
            AuthorId = authorId,
            PostId = postId
        };
    }

    private static MessageCreateUpdateDto ValidMessage(int senderId, int receiverId)
    {
        return new MessageCreateUpdateDto
        {
            Content = "Valid message",
            SenderId = senderId,
            ReceiverId = receiverId
        };
    }

    private static ReportCreateUpdateDto ValidReport(int reporterId, int postId)
    {
        return new ReportCreateUpdateDto
        {
            Reason = ReportReason.Other,
            Status = ReportStatus.Pending,
            ReporterId = reporterId,
            PostId = postId
        };
    }

    private static LikeCreateUpdateDto ValidLike(int userId, int postId)
    {
        return new LikeCreateUpdateDto
        {
            UserId = userId,
            PostId = postId
        };
    }

    private static FollowCreateUpdateDto ValidFollow(int followerId, int followingId)
    {
        return new FollowCreateUpdateDto
        {
            FollowerId = followerId,
            FollowingId = followingId
        };
    }

    private static NotificationCreateUpdateDto ValidNotification(int recipientId)
    {
        return new NotificationCreateUpdateDto
        {
            Message = "Valid notification",
            RecipientId = recipientId
        };
    }
}
