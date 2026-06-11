using System.ComponentModel.DataAnnotations;
using Filip_Rados_lab5.Models;

namespace Filip_Rados_lab5.ViewModels
{
    public class UserFormModel
    {
        public int Id { get; set; }

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

        [StringLength(11, MinimumLength = 11, ErrorMessage = "OIB mora imati 11 znamenki.")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "OIB mora sadrzavati samo znamenke.")]
        public string? OIB { get; set; }

        [StringLength(13, MinimumLength = 13, ErrorMessage = "JMBG mora imati 13 znamenki.")]
        [RegularExpression(@"^\d{13}$", ErrorMessage = "JMBG mora sadrzavati samo znamenke.")]
        public string? JMBG { get; set; }
    }

    public class TagFormModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Naziv taga je obvezan.")]
        [StringLength(40, MinimumLength = 2, ErrorMessage = "Naziv mora imati izmedu 2 i 40 znakova.")]
        public string Name { get; set; } = string.Empty;
    }

    public class PostFormModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Naslov je obvezan.")]
        [StringLength(120, MinimumLength = 5, ErrorMessage = "Naslov mora imati izmedu 5 i 120 znakova.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Sadrzaj je obvezan.")]
        [StringLength(4000, MinimumLength = 10, ErrorMessage = "Sadrzaj mora imati izmedu 10 i 4000 znakova.")]
        public string Content { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kategorija je obvezna.")]
        public PostCategory? Category { get; set; }

        [Required(ErrorMessage = "Datum objave je obvezan.")]
        public DateTime? CreatedAt { get; set; }

        public int? AuthorId { get; set; }

        public string? AuthorText { get; set; }

        [Display(Name = "Tagovi")]
        public List<int> TagIds { get; set; } = new();
    }

    public class CommentFormModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Komentar je obvezan.")]
        [StringLength(1200, MinimumLength = 3, ErrorMessage = "Komentar mora imati izmedu 3 i 1200 znakova.")]
        public string Content { get; set; } = string.Empty;

        [Required(ErrorMessage = "Datum komentara je obvezan.")]
        public DateTime? CreatedAt { get; set; }

        [Required(ErrorMessage = "Autor je obvezan.")]
        public int? AuthorId { get; set; }

        public string? AuthorText { get; set; }

        [Required(ErrorMessage = "Post je obvezan.")]
        public int? PostId { get; set; }

        public string? PostText { get; set; }
    }

    public class MessageFormModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Poruka je obvezna.")]
        [StringLength(2000, MinimumLength = 2, ErrorMessage = "Poruka mora imati izmedu 2 i 2000 znakova.")]
        public string Content { get; set; } = string.Empty;

        [Required(ErrorMessage = "Datum slanja je obvezan.")]
        public DateTime? SentAt { get; set; }

        public bool IsRead { get; set; }

        [Required(ErrorMessage = "Posiljatelj je obvezan.")]
        public int? SenderId { get; set; }

        public string? SenderText { get; set; }

        [Required(ErrorMessage = "Primatelj je obvezan.")]
        public int? ReceiverId { get; set; }

        public string? ReceiverText { get; set; }
    }

    public class ReportFormModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Razlog prijave je obvezan.")]
        public ReportReason? Reason { get; set; }

        [Required(ErrorMessage = "Status prijave je obvezan.")]
        public ReportStatus? Status { get; set; }

        [Required(ErrorMessage = "Datum prijave je obvezan.")]
        public DateTime? CreatedAt { get; set; }

        [Required(ErrorMessage = "Prijavitelj je obvezan.")]
        public int? ReporterId { get; set; }

        public string? ReporterText { get; set; }

        [Required(ErrorMessage = "Post je obvezan.")]
        public int? PostId { get; set; }

        public string? PostText { get; set; }
    }

    public class LikeFormModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Korisnik je obvezan.")]
        public int? UserId { get; set; }

        public string? UserText { get; set; }

        [Required(ErrorMessage = "Post je obvezan.")]
        public int? PostId { get; set; }

        public string? PostText { get; set; }
    }

    public class FollowFormModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Korisnik koji prati je obvezan.")]
        public int? FollowerId { get; set; }

        public string? FollowerText { get; set; }

        [Required(ErrorMessage = "Korisnik koji se prati je obvezan.")]
        public int? FollowingId { get; set; }

        public string? FollowingText { get; set; }
    }

    public class NotificationFormModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Poruka obavijesti je obvezna.")]
        [StringLength(800, MinimumLength = 3, ErrorMessage = "Poruka mora imati izmedu 3 i 800 znakova.")]
        public string Message { get; set; } = string.Empty;

        [Required(ErrorMessage = "Datum obavijesti je obvezan.")]
        public DateTime? CreatedAt { get; set; }

        [Required(ErrorMessage = "Primatelj je obvezan.")]
        public int? RecipientId { get; set; }

        public string? RecipientText { get; set; }
    }
}
