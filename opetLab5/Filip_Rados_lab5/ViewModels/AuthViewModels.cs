using System.ComponentModel.DataAnnotations;

namespace Filip_Rados_lab5.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username or email is required.")]
        public string UsernameOrEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Ime je obvezno.")]
        [StringLength(60, MinimumLength = 2, ErrorMessage = "Ime mora imati izmedu 2 i 60 znakova.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Prezime je obvezno.")]
        [StringLength(80, MinimumLength = 2, ErrorMessage = "Prezime mora imati izmedu 2 i 80 znakova.")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Username is required.")]
        [StringLength(40, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 40 characters.")]
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

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must contain at least 6 characters.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Potvrda lozinke je obvezna.")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class AuthPageViewModel
    {
        public LoginViewModel Login { get; set; } = new();
        public RegisterViewModel Register { get; set; } = new();
        public string ActiveTab { get; set; } = "login";
    }
}
