using Filip_Rados_lab5.Models;

namespace Filip_Rados_lab5.Dtos
{
    public static class ApiDtoMapper
    {
        public static UserDto ToDto(this User user)
        {
            return new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                DateOfBirth = user.DateOfBirth,
                PostCount = user.Posts?.Count ?? 0,
                FollowerCount = user.Followers?.Count ?? 0,
                FollowingCount = user.Following?.Count ?? 0,
                CurrentProfileImage = user.ProfileImages?
                    .OrderByDescending(image => image.CreatedAt)
                    .FirstOrDefault()
                    ?.ToDto(),
                ProfileImages = user.ProfileImages?
                    .OrderByDescending(image => image.CreatedAt)
                    .Select(image => image.ToDto())
                    .ToList() ?? new List<UserProfileImageDto>()
            };
        }

        public static UserSummaryDto ToSummaryDto(this User user)
        {
            return new UserSummaryDto
            {
                Id = user.Id,
                Username = user.UserName ?? string.Empty,
                DisplayName = $"{user.FirstName} {user.LastName}".Trim(),
                ProfileImagePath = user.ProfileImages?
                    .OrderByDescending(image => image.CreatedAt)
                    .FirstOrDefault()
                    ?.FilePath
            };
        }

        public static TagDto ToDto(this Tag tag)
        {
            return new TagDto
            {
                Id = tag.Id,
                Name = tag.Name,
                PostCount = tag.Posts?.Count ?? 0
            };
        }

        public static PostDto ToDto(this Post post)
        {
            return new PostDto
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                Category = post.Category,
                CreatedAt = post.CreatedAt,
                Author = post.Author?.ToSummaryDto(),
                Tags = post.Tags.Select(tag => tag.ToDto()).ToList(),
                Videos = post.Videos?
                    .OrderByDescending(video => video.CreatedAt)
                    .Select(video => video.ToDto())
                    .ToList() ?? new List<PostVideoDto>(),
                CommentCount = post.Comments?.Count ?? 0,
                LikeCount = post.Likes?.Count ?? 0
            };
        }

        public static PostSummaryDto ToSummaryDto(this Post post)
        {
            return new PostSummaryDto
            {
                Id = post.Id,
                Title = post.Title,
                Category = post.Category
            };
        }

        public static CommentDto ToDto(this Comment comment)
        {
            return new CommentDto
            {
                Id = comment.Id,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                Author = comment.Author?.ToSummaryDto(),
                Post = comment.Post?.ToSummaryDto()
            };
        }

        public static MessageDto ToDto(this Message message)
        {
            return new MessageDto
            {
                Id = message.Id,
                Content = message.Content,
                SentAt = message.SentAt,
                IsRead = message.IsRead,
                Sender = message.Sender?.ToSummaryDto(),
                Receiver = message.Receiver?.ToSummaryDto()
            };
        }

        public static ReportDto ToDto(this Report report)
        {
            return new ReportDto
            {
                Id = report.Id,
                Reason = report.Reason,
                Status = report.Status,
                CreatedAt = report.CreatedAt,
                Reporter = report.Reporter?.ToSummaryDto(),
                Post = report.Post?.ToSummaryDto()
            };
        }

        public static LikeDto ToDto(this Like like)
        {
            return new LikeDto
            {
                Id = like.Id,
                User = like.User?.ToSummaryDto(),
                Post = like.Post?.ToSummaryDto()
            };
        }

        public static FollowDto ToDto(this Follow follow)
        {
            return new FollowDto
            {
                Id = follow.Id,
                Follower = follow.Follower?.ToSummaryDto(),
                Following = follow.Following?.ToSummaryDto()
            };
        }

        public static NotificationDto ToDto(this Notification notification)
        {
            return new NotificationDto
            {
                Id = notification.Id,
                Message = notification.Message,
                CreatedAt = notification.CreatedAt,
                Recipient = notification.Recipient?.ToSummaryDto()
            };
        }

        public static PostVideoDto ToDto(this PostVideo video)
        {
            return new PostVideoDto
            {
                Id = video.Id,
                FileName = video.FileName,
                FilePath = video.FilePath,
                ContentType = video.ContentType,
                FileSize = video.FileSize,
                CreatedAt = video.CreatedAt
            };
        }

        public static UserProfileImageDto ToDto(this UserProfileImage image)
        {
            return new UserProfileImageDto
            {
                Id = image.Id,
                FileName = image.FileName,
                FilePath = image.FilePath,
                ContentType = image.ContentType,
                FileSize = image.FileSize,
                CreatedAt = image.CreatedAt
            };
        }
    }
}
