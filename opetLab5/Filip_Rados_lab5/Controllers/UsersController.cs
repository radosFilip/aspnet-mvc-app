using Filip_Rados_lab5.Data;
using Filip_Rados_lab5.Models;
using Filip_Rados_lab5.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Filip_Rados_lab5.Controllers
{
    public class UsersController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly UserManager<User> _userManager;

        public UsersController(AppDbContext context, IWebHostEnvironment environment, UserManager<User> userManager)
        {
            _context = context;
            _environment = environment;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string? q)
        {
            var users = await BuildQuery(q).ToListAsync();
            ViewBag.Query = q;
            return View(users);
        }

        public async Task<IActionResult> Search(string? q)
        {
            var users = await BuildQuery(q).ToListAsync();
            return PartialView("_List", users);
        }

        public async Task<IActionResult> Autocomplete(string? term)
        {
            term = term?.Trim();

            var users = await _context.Users
                .Where(user => string.IsNullOrEmpty(term)
                    || EF.Functions.Like(user.UserName, $"%{term}%")
                    || EF.Functions.Like(user.FirstName, $"%{term}%")
                    || EF.Functions.Like(user.LastName, $"%{term}%")
                    || EF.Functions.Like(user.Email, $"%{term}%"))
                .OrderBy(user => user.UserName)
                .Take(12)
                .Select(user => new
                {
                    id = user.Id,
                    text = "@" + user.UserName,
                    subtitle = user.FirstName + " " + user.LastName + " · " + user.Email
                })
                .ToListAsync();

            return Json(users);
        }

        public async Task<IActionResult> Details(int id)
        {
            var user = await _context.Users
                .Include(item => item.ProfileImages)
                .FirstOrDefaultAsync(item => item.Id == id);
            if (user == null) return NotFound();

            ViewBag.Posts = await _context.Posts
                .Include(post => post.Tags)
                .Include(post => post.Comments)
                .Include(post => post.Likes)
                .Where(post => post.AuthorId == id)
                .OrderByDescending(post => post.CreatedAt)
                .ToListAsync();

            ViewBag.Comments = await _context.Comments
                .Include(comment => comment.Post)
                .Where(comment => comment.AuthorId == id)
                .OrderByDescending(comment => comment.CreatedAt)
                .ToListAsync();

            ViewBag.Following = await _context.Follows
                .Include(follow => follow.Following)
                    .ThenInclude(user => user.ProfileImages)
                .Where(follow => follow.FollowerId == id)
                .Select(follow => follow.Following)
                .ToListAsync();

            ViewBag.Followers = await _context.Follows
                .Include(follow => follow.Follower)
                    .ThenInclude(user => user.ProfileImages)
                .Where(follow => follow.FollowingId == id)
                .Select(follow => follow.Follower)
                .ToListAsync();

            return View(user);
        }

        public async Task<IActionResult> GetProfileImages(int userId)
        {
            if (!await _context.Users.AnyAsync(user => user.Id == userId))
            {
                return NotFound();
            }

            var images = await _context.UserProfileImages
                .Where(image => image.UserId == userId)
                .OrderByDescending(image => image.CreatedAt)
                .ToListAsync();

            return PartialView("_ProfileImageList", images);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadProfileImage(int userId, IFormFile file)
        {
            if (!await _context.Users.AnyAsync(user => user.Id == userId))
            {
                return NotFound();
            }
            if (!CanManageUser(userId)) return Forbid();

            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "Odaberite sliku." });
            }

            if (!IsAllowedProfileImage(file))
            {
                return BadRequest(new { message = "Dopustene su JPG, PNG, WebP i GIF slike do 5 MB." });
            }

            await SaveProfileImage(userId, file);

            return Json(new { success = true });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProfileImage(int id)
        {
            var image = await _context.UserProfileImages.FindAsync(id);
            if (image == null) return NotFound();
            if (!CanManageUser(image.UserId)) return Forbid();

            DeletePhysicalFile(image.FilePath);

            _context.UserProfileImages.Remove(image);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View(new UserFormModel
            {
                DateOfBirth = DateTime.Today.AddYears(-18)
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserFormModel model, IFormFile? profileImageFile)
        {
            await ValidateUniqueUser(model);

            if (profileImageFile != null && profileImageFile.Length > 0 && !IsAllowedProfileImage(profileImageFile))
            {
                ModelState.AddModelError(nameof(profileImageFile), "Dopustene su JPG, PNG, WebP i GIF slike do 5 MB.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new User();
            MapToEntity(model, user);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            if (profileImageFile != null && profileImageFile.Length > 0)
            {
                await SaveProfileImage(user.Id, profileImageFile);
            }

            TempData["Flash"] = "Korisnik je uspjesno kreiran.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            if (!CanManageUser(id)) return Forbid();

            return View(MapToForm(user));
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserFormModel model)
        {
            if (id != model.Id) return NotFound();

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            if (!CanManageUser(id)) return Forbid();

            await ValidateUniqueUser(model);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            MapToEntity(model, user);
            await _context.SaveChangesAsync();

            TempData["Flash"] = "Korisnik je uspjesno azuriran.";
            return RedirectToAction(nameof(Details), new { id = user.Id });
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            ViewBag.DeleteBlockedReason = await GetDeleteBlockedReason(id);
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            var blockedReason = await GetDeleteBlockedReason(id);
            if (blockedReason != null)
            {
                ModelState.AddModelError(string.Empty, blockedReason);
                ViewBag.DeleteBlockedReason = blockedReason;
                return View(user);
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            DeleteUploadDirectory(Path.Combine(_environment.WebRootPath, "uploads", "users", id.ToString()));

            TempData["Flash"] = "Korisnik je obrisan.";
            return RedirectToAction(nameof(Index));
        }

        private static bool IsAllowedProfileImage(IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
            const long maxSize = 5 * 1024 * 1024;

            return file.Length <= maxSize &&
                   file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase) &&
                   allowedExtensions.Contains(extension);
        }

        private async Task SaveProfileImage(int userId, IFormFile file)
        {
            var relativeDirectory = Path.Combine("uploads", "users", userId.ToString(), "profile-images");
            var uploadsPath = Path.Combine(_environment.WebRootPath, relativeDirectory);
            Directory.CreateDirectory(uploadsPath);

            var storedFileName = Guid.NewGuid().ToString("N") + Path.GetExtension(file.FileName);
            var physicalPath = Path.Combine(uploadsPath, storedFileName);

            await using (var stream = new FileStream(physicalPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var image = new UserProfileImage
            {
                UserId = userId,
                FileName = Path.GetFileName(file.FileName),
                FilePath = "/" + relativeDirectory.Replace(Path.DirectorySeparatorChar, '/') + "/" + storedFileName,
                ContentType = file.ContentType,
                FileSize = file.Length,
                CreatedAt = DateTime.UtcNow
            };

            _context.UserProfileImages.Add(image);
            await _context.SaveChangesAsync();
        }

        private void DeletePhysicalFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath)) return;

            var relativePath = filePath.TrimStart('/', '\\')
                .Replace('/', Path.DirectorySeparatorChar)
                .Replace('\\', Path.DirectorySeparatorChar);
            var physicalPath = Path.GetFullPath(Path.Combine(_environment.WebRootPath, relativePath));
            var webRootPath = Path.GetFullPath(_environment.WebRootPath);

            if (physicalPath.StartsWith(webRootPath, StringComparison.OrdinalIgnoreCase) &&
                System.IO.File.Exists(physicalPath))
            {
                System.IO.File.Delete(physicalPath);
            }
        }

        private static void DeleteUploadDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, recursive: true);
            }
        }

        private IQueryable<User> BuildQuery(string? q)
        {
            q = q?.Trim();

            var query = _context.Users
                .Include(user => user.Posts)
                .Include(user => user.ProfileImages)
                .AsQueryable();

            if (!string.IsNullOrEmpty(q))
            {
                query = query.Where(user =>
                    EF.Functions.Like(user.UserName, $"%{q}%") ||
                    EF.Functions.Like(user.FirstName, $"%{q}%") ||
                    EF.Functions.Like(user.LastName, $"%{q}%") ||
                    EF.Functions.Like(user.Email, $"%{q}%"));
            }

            return query.OrderBy(user => user.UserName);
        }

        private async Task ValidateUniqueUser(UserFormModel model)
        {
            if (await _context.Users.AnyAsync(user => user.Id != model.Id && user.UserName == model.Username))
            {
                ModelState.AddModelError(nameof(model.Username), "Korisnicko ime je vec zauzeto.");
            }

            if (await _context.Users.AnyAsync(user => user.Id != model.Id && user.Email == model.Email))
            {
                ModelState.AddModelError(nameof(model.Email), "Email je vec zauzet.");
            }
        }

        private async Task<string?> GetDeleteBlockedReason(int id)
        {
            var hasRelatedData =
                await _context.Posts.AnyAsync(post => post.AuthorId == id) ||
                await _context.Comments.AnyAsync(comment => comment.AuthorId == id) ||
                await _context.Messages.AnyAsync(message => message.SenderId == id || message.ReceiverId == id) ||
                await _context.Reports.AnyAsync(report => report.ReporterId == id) ||
                await _context.Likes.AnyAsync(like => like.UserId == id) ||
                await _context.Follows.AnyAsync(follow => follow.FollowerId == id || follow.FollowingId == id) ||
                await _context.Notifications.AnyAsync(notification => notification.RecipientId == id);

            return hasRelatedData
                ? "Korisnik ima povezane zapise pa ga nije moguce obrisati bez gubitka podataka."
                : null;
        }

        private static UserFormModel MapToForm(User user)
        {
            return new UserFormModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                DateOfBirth = user.DateOfBirth,
                OIB = user.OIB,
                JMBG = user.JMBG
            };
        }

        private static void MapToEntity(UserFormModel model, User user)
        {
            user.FirstName = model.FirstName.Trim();
            user.LastName = model.LastName.Trim();
            user.UserName = model.Username.Trim();
            user.Email = model.Email.Trim();
            user.DateOfBirth = model.DateOfBirth!.Value;
            user.OIB = string.IsNullOrWhiteSpace(model.OIB) ? null : model.OIB.Trim();
            user.JMBG = string.IsNullOrWhiteSpace(model.JMBG) ? null : model.JMBG.Trim();
        }

        private bool CanManageUser(int userId)
        {
            var currentUserId = _userManager.GetUserId(User);
            return User.IsInRole("Admin") || userId.ToString() == currentUserId;
        }
    }
}
