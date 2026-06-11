using System.ComponentModel.DataAnnotations;
using Filip_Rados_lab5.Models;

namespace Filip_Rados_lab5.Dtos
{
    public class UserDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public int PostCount { get; set; }
        public int FollowerCount { get; set; }
        public int FollowingCount { get; set; }
        public UserProfileImageDto? CurrentProfileImage { get; set; }
        public List<UserProfileImageDto> ProfileImages { get; set; } = new();
    }

    public class UserCreateUpdateDto
    {
        [Required(ErrorMessage = "Ime je obvezno.")]
        [StringLength(60, MinimumLength = 2, ErrorMessage = "Ime mora imati izmedu 2 i 60 znakova.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Prezime je obvezno.")]
        [StringLength(80, MinimumLength = 2, ErrorMessage = "Prezime mora imati izmedu 2 i 80 znakova.")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Korisnicko ime je obvezno.")]
        [StringLength(40, MinimumLength = 3, ErrorMessage = "Korisnicko ime mora imati izmedu 3 i 40 znakova.")]
        [RegularExpression(@"^[A-Za-z0-9_.-]+$", ErrorMessage = "Koristite samo slova, brojeve, tocku, crticu ili donju crtu.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email je obvezan.")]
        [EmailAddress(ErrorMessage = "Unesite ispravnu email adresu.")]
        [StringLength(120, ErrorMessage = "Email moze imati najvise 120 znakova.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Datum rodenja je obvezan.")]
        public DateTime? DateOfBirth { get; set; }
    }

    public class TagDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int PostCount { get; set; }
    }

    public class TagCreateUpdateDto
    {
        [Required(ErrorMessage = "Naziv taga je obvezan.")]
        [StringLength(40, MinimumLength = 2, ErrorMessage = "Naziv mora imati izmedu 2 i 40 znakova.")]
        public string Name { get; set; } = string.Empty;
    }

    public class PostDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public PostCategory Category { get; set; }
        public DateTime CreatedAt { get; set; }
        public UserSummaryDto? Author { get; set; }
        public List<TagDto> Tags { get; set; } = new();
        public List<PostVideoDto> Videos { get; set; } = new();
        public int CommentCount { get; set; }
        public int LikeCount { get; set; }
    }

    public class PostCreateUpdateDto
    {
        [Required(ErrorMessage = "Naslov je obvezan.")]
        [StringLength(120, MinimumLength = 5, ErrorMessage = "Naslov mora imati izmedu 5 i 120 znakova.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Sadrzaj je obvezan.")]
        [StringLength(4000, MinimumLength = 10, ErrorMessage = "Sadrzaj mora imati izmedu 10 i 4000 znakova.")]
        public string Content { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kategorija je obvezna.")]
        public PostCategory? Category { get; set; }

        public DateTime? CreatedAt { get; set; }

        [Required(ErrorMessage = "Autor je obvezan.")]
        public int? AuthorId { get; set; }

        public List<int> TagIds { get; set; } = new();
    }

    public class CommentDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public UserSummaryDto? Author { get; set; }
        public PostSummaryDto? Post { get; set; }
    }

    public class CommentCreateUpdateDto
    {
        [Required(ErrorMessage = "Komentar je obvezan.")]
        [StringLength(1200, MinimumLength = 3, ErrorMessage = "Komentar mora imati izmedu 3 i 1200 znakova.")]
        public string Content { get; set; } = string.Empty;

        public DateTime? CreatedAt { get; set; }

        [Required(ErrorMessage = "Autor je obvezan.")]
        public int? AuthorId { get; set; }

        [Required(ErrorMessage = "Post je obvezan.")]
        public int? PostId { get; set; }
    }

    public class MessageDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
        public bool IsRead { get; set; }
        public UserSummaryDto? Sender { get; set; }
        public UserSummaryDto? Receiver { get; set; }
    }

    public class MessageCreateUpdateDto
    {
        [Required(ErrorMessage = "Poruka je obvezna.")]
        [StringLength(2000, MinimumLength = 2, ErrorMessage = "Poruka mora imati izmedu 2 i 2000 znakova.")]
        public string Content { get; set; } = string.Empty;

        public DateTime? SentAt { get; set; }
        public bool IsRead { get; set; }

        [Required(ErrorMessage = "Posiljatelj je obvezan.")]
        public int? SenderId { get; set; }

        [Required(ErrorMessage = "Primatelj je obvezan.")]
        public int? ReceiverId { get; set; }
    }

    public class ReportDto
    {
        public int Id { get; set; }
        public ReportReason Reason { get; set; }
        public ReportStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public UserSummaryDto? Reporter { get; set; }
        public PostSummaryDto? Post { get; set; }
    }

    public class ReportCreateUpdateDto
    {
        [Required(ErrorMessage = "Razlog prijave je obvezan.")]
        public ReportReason? Reason { get; set; }

        [Required(ErrorMessage = "Status prijave je obvezan.")]
        public ReportStatus? Status { get; set; }

        public DateTime? CreatedAt { get; set; }

        [Required(ErrorMessage = "Prijavitelj je obvezan.")]
        public int? ReporterId { get; set; }

        [Required(ErrorMessage = "Post je obvezan.")]
        public int? PostId { get; set; }
    }

    public class LikeDto
    {
        public int Id { get; set; }
        public UserSummaryDto? User { get; set; }
        public PostSummaryDto? Post { get; set; }
    }

    public class LikeCreateUpdateDto
    {
        [Required(ErrorMessage = "Korisnik je obvezan.")]
        public int? UserId { get; set; }

        [Required(ErrorMessage = "Post je obvezan.")]
        public int? PostId { get; set; }
    }

    public class FollowDto
    {
        public int Id { get; set; }
        public UserSummaryDto? Follower { get; set; }
        public UserSummaryDto? Following { get; set; }
    }

    public class FollowCreateUpdateDto
    {
        [Required(ErrorMessage = "Korisnik koji prati je obvezan.")]
        public int? FollowerId { get; set; }

        [Required(ErrorMessage = "Korisnik koji se prati je obvezan.")]
        public int? FollowingId { get; set; }
    }

    public class NotificationDto
    {
        public int Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public UserSummaryDto? Recipient { get; set; }
    }

    public class NotificationCreateUpdateDto
    {
        [Required(ErrorMessage = "Poruka obavijesti je obvezna.")]
        [StringLength(800, MinimumLength = 3, ErrorMessage = "Poruka mora imati izmedu 3 i 800 znakova.")]
        public string Message { get; set; } = string.Empty;

        public DateTime? CreatedAt { get; set; }

        [Required(ErrorMessage = "Primatelj je obvezan.")]
        public int? RecipientId { get; set; }
    }

    public class UserSummaryDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string? ProfileImagePath { get; set; }
    }

    public class PostSummaryDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public PostCategory Category { get; set; }
    }

    public class PostVideoDto
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UserProfileImageDto
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
