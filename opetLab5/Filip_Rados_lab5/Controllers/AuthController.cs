using Filip_Rados_lab5.Models;
using Filip_Rados_lab5.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Filip_Rados_lab5.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        [AllowAnonymous]
        public IActionResult Index(string tab = "login")
        {
            return View(new AuthPageViewModel
            {
                ActiveTab = tab == "register" ? "register" : "login",
                Register = new RegisterViewModel
                {
                    DateOfBirth = DateTime.Today.AddYears(-18)
                }
            });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind(Prefix = "Login")] LoginViewModel login, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", new AuthPageViewModel
                {
                    ActiveTab = "login",
                    Login = login,
                    Register = NewRegisterModel()
                });
            }

            var loginName = login.UsernameOrEmail.Trim();
            var user = loginName.Contains('@')
                ? await _userManager.FindByEmailAsync(loginName)
                : await _userManager.FindByNameAsync(loginName);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Neispravno korisnicko ime/email ili lozinka.");
                return View("Index", new AuthPageViewModel
                {
                    ActiveTab = "login",
                    Login = login,
                    Register = NewRegisterModel()
                });
            }

            var result = await _signInManager.PasswordSignInAsync(user, login.Password, login.RememberMe, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Neispravno korisnicko ime/email ili lozinka.");
                return View("Index", new AuthPageViewModel
                {
                    ActiveTab = "login",
                    Login = login,
                    Register = NewRegisterModel()
                });
            }

            return LocalRedirect(SafeReturnUrl(returnUrl));
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string? returnUrl = null)
        {
            if (!IsSupportedProvider(provider) || !IsExternalProviderConfigured(provider))
            {
                ModelState.AddModelError(string.Empty, $"{provider} prijava nije konfigurirana.");
                return View("Index", new AuthPageViewModel
                {
                    ActiveTab = "login",
                    Register = NewRegisterModel()
                });
            }

            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Auth", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
        {
            if (!string.IsNullOrWhiteSpace(remoteError))
            {
                ModelState.AddModelError(string.Empty, $"Vanjska prijava nije uspjela: {remoteError}");
                return View("Index", new AuthPageViewModel
                {
                    ActiveTab = "login",
                    Register = NewRegisterModel()
                });
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ModelState.AddModelError(string.Empty, "Nije moguce dohvatiti podatke vanjske prijave.");
                return View("Index", new AuthPageViewModel
                {
                    ActiveTab = "login",
                    Register = NewRegisterModel()
                });
            }

            var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            if (signInResult.Succeeded)
            {
                return LocalRedirect(SafeReturnUrl(returnUrl));
            }

            var externalEmail = info.Principal.FindFirstValue(ClaimTypes.Email);
            var user = string.IsNullOrWhiteSpace(externalEmail)
                ? null
                : await _userManager.FindByEmailAsync(externalEmail);
            if (user == null)
            {
                var fallbackEmail = BuildExternalFallbackEmail(info.LoginProvider, info.ProviderKey);
                var accountEmail = string.IsNullOrWhiteSpace(externalEmail)
                    ? fallbackEmail
                    : externalEmail;
                var usernameSource = string.IsNullOrWhiteSpace(externalEmail)
                    ? $"{info.LoginProvider}_{info.ProviderKey}"
                    : externalEmail;

                user = new User
                {
                    FirstName = info.Principal.FindFirstValue(ClaimTypes.GivenName) ?? info.LoginProvider,
                    LastName = info.Principal.FindFirstValue(ClaimTypes.Surname) ?? "User",
                    UserName = await GenerateUniqueUsername(usernameSource),
                    Email = accountEmail,
                    EmailConfirmed = !string.IsNullOrWhiteSpace(externalEmail),
                    DateOfBirth = DateTime.Today.AddYears(-18)
                };

                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    AddIdentityErrors(createResult);
                    return View("Index", new AuthPageViewModel
                    {
                        ActiveTab = "login",
                        Register = NewRegisterModel()
                    });
                }

                await _userManager.AddToRoleAsync(user, "User");
            }

            var addLoginResult = await _userManager.AddLoginAsync(user, info);
            if (!addLoginResult.Succeeded && !addLoginResult.Errors.Any(error => error.Code == "LoginAlreadyAssociated"))
            {
                AddIdentityErrors(addLoginResult);
                return View("Index", new AuthPageViewModel
                {
                    ActiveTab = "login",
                    Register = NewRegisterModel()
                });
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            return LocalRedirect(SafeReturnUrl(returnUrl));
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind(Prefix = "Register")] RegisterViewModel register, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", new AuthPageViewModel
                {
                    ActiveTab = "register",
                    Register = register
                });
            }

            var username = register.Username.Trim();
            var email = register.Email.Trim();
            var user = new User
            {
                FirstName = register.FirstName.Trim(),
                LastName = register.LastName.Trim(),
                Username = username,
                UserName = username,
                Email = email,
                DateOfBirth = register.DateOfBirth!.Value,
                OIB = string.IsNullOrWhiteSpace(register.OIB) ? null : register.OIB.Trim(),
                JMBG = string.IsNullOrWhiteSpace(register.JMBG) ? null : register.JMBG.Trim()
            };

            var result = await _userManager.CreateAsync(user, register.Password);
            if (!result.Succeeded)
            {
                AddIdentityErrors(result);
                return View("Index", new AuthPageViewModel
                {
                    ActiveTab = "register",
                    Register = register
                });
            }

            await _userManager.AddToRoleAsync(user, "User");
            await _signInManager.SignInAsync(user, isPersistent: false);

            TempData["Flash"] = "Account created successfully.";
            return LocalRedirect(SafeReturnUrl(returnUrl));
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            return RedirectToAction("Index", "Home");
        }

        private void AddIdentityErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private string SafeReturnUrl(string? returnUrl)
        {
            return !string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl)
                ? returnUrl
                : Url.Action("Index", "Home") ?? "/";
        }

        private static RegisterViewModel NewRegisterModel()
        {
            return new RegisterViewModel
            {
                DateOfBirth = DateTime.Today.AddYears(-18)
            };
        }

        private static bool IsSupportedProvider(string provider)
        {
            return provider == "Google" || provider == "Facebook";
        }

        private bool IsExternalProviderConfigured(string provider)
        {
            return provider switch
            {
                "Google" => !string.IsNullOrWhiteSpace(_configuration["Authentication:Google:ClientId"]) &&
                            !string.IsNullOrWhiteSpace(_configuration["Authentication:Google:ClientSecret"]),
                "Facebook" => !string.IsNullOrWhiteSpace(_configuration["Authentication:Facebook:AppId"]) &&
                              !string.IsNullOrWhiteSpace(_configuration["Authentication:Facebook:AppSecret"]),
                _ => false
            };
        }

        private async Task<string> GenerateUniqueUsername(string email)
        {
            var baseUsername = new string(email.Split('@')[0]
                .Where(character => char.IsLetterOrDigit(character) || character == '_' || character == '-' || character == '.')
                .ToArray());

            if (string.IsNullOrWhiteSpace(baseUsername))
            {
                baseUsername = "google_user";
            }

            var username = baseUsername;
            var suffix = 1;
            while (await _userManager.FindByNameAsync(username) != null)
            {
                username = $"{baseUsername}{suffix}";
                suffix++;
            }

            return username;
        }

        private static string BuildExternalFallbackEmail(string provider, string providerKey)
        {
            var safeProvider = new string(provider
                .ToLowerInvariant()
                .Where(char.IsLetterOrDigit)
                .ToArray());
            var safeProviderKey = new string(providerKey
                .ToLowerInvariant()
                .Where(char.IsLetterOrDigit)
                .ToArray());

            if (string.IsNullOrWhiteSpace(safeProvider))
            {
                safeProvider = "external";
            }

            if (string.IsNullOrWhiteSpace(safeProviderKey))
            {
                safeProviderKey = Guid.NewGuid().ToString("N");
            }

            return $"{safeProvider}_{safeProviderKey}@external.local";
        }
    }
}
