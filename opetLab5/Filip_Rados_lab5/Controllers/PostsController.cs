using Filip_Rados_lab5.Data;
using Filip_Rados_lab5.Models;
using Filip_Rados_lab5.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Filip_Rados_lab5.Controllers
{
    public class PostsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly UserManager<User> _userManager;

        public PostsController(AppDbContext context, IWebHostEnvironment environment, UserManager<User> userManager)
        {
            _context = context;
            _environment = environment;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string? category, string? q)
        {
            var posts = await BuildQuery(category, q).ToListAsync();
            ViewBag.ActiveCategory = category;
            ViewBag.Query = q;
            return View(posts);
        }

        public async Task<IActionResult> Search(string? category, string? q)
        {
            var posts = await BuildQuery(category, q).ToListAsync();
            return PartialView("_List", posts);
        }

        public async Task<IActionResult> Autocomplete(string? term)
        {
            term = term?.Trim();

            var posts = await _context.Posts
                .Include(post => post.Author)
                .Where(post => string.IsNullOrEmpty(term)
                    || EF.Functions.Like(post.Title, $"%{term}%")
                    || EF.Functions.Like(post.Author.UserName, $"%{term}%"))
                .OrderByDescending(post => post.CreatedAt)
                .Take(12)
                .Select(post => new
                {
                    id = post.Id,
                    text = post.Title,
                    subtitle = "@" + post.Author.UserName + " · " + post.Category
                })
                .ToListAsync();

            return Json(posts);
        }

        public async Task<IActionResult> Details(int id)
        {
            var post = await _context.Posts
                .Include(item => item.Author)
                    .ThenInclude(author => author.ProfileImages)
                .Include(item => item.Comments)
                    .ThenInclude(comment => comment.Author)
                .Include(item => item.Tags)
                .Include(item => item.Likes)
                .Include(item => item.Videos)
                .FirstOrDefaultAsync(item => item.Id == id);

            if (post == null) return NotFound();

            ViewBag.LikeCount = post.Likes.Count;
            ViewBag.HasLiked = User.Identity?.IsAuthenticated == true &&
                await _context.Likes.AnyAsync(like => like.PostId == id && like.UserId == GetCurrentUserId());
            return View(post);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleLike(int id)
        {
            var userId = GetCurrentUserId();
            var post = await _context.Posts
                .Include(item => item.Author)
                .FirstOrDefaultAsync(item => item.Id == id);

            if (post == null) return NotFound();

            var existingLike = await _context.Likes
                .FirstOrDefaultAsync(like => like.PostId == id && like.UserId == userId);

            if (existingLike == null)
            {
                _context.Likes.Add(new Like
                {
                    PostId = id,
                    UserId = userId
                });

                await AddNotification(
                    post.AuthorId,
                    userId,
                    $"{GetDisplayName()} liked your post \"{post.Title}\".");

                TempData["Flash"] = "Post liked.";
            }
            else
            {
                _context.Likes.Remove(existingLike);
                TempData["Flash"] = "Like removed.";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int id, string content)
        {
            content = content?.Trim() ?? string.Empty;
            if (content.Length < 3)
            {
                TempData["Flash"] = "Comment must contain at least 3 characters.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var userId = GetCurrentUserId();
            var post = await _context.Posts
                .Include(item => item.Author)
                .FirstOrDefaultAsync(item => item.Id == id);

            if (post == null) return NotFound();

            _context.Comments.Add(new Comment
            {
                PostId = id,
                AuthorId = userId,
                Content = content,
                CreatedAt = DateTime.Now
            });

            await AddNotification(
                post.AuthorId,
                userId,
                $"{GetDisplayName()} commented on your post \"{post.Title}\".");

            await _context.SaveChangesAsync();
            TempData["Flash"] = "Comment added.";
            return RedirectToAction(nameof(Details), new { id });
        }

        public async Task<IActionResult> GetVideos(int postId)
        {
            if (!await _context.Posts.AnyAsync(post => post.Id == postId))
            {
                return NotFound();
            }

            var videos = await _context.PostVideos
                .Where(video => video.PostId == postId)
                .OrderByDescending(video => video.CreatedAt)
                .ToListAsync();

            return PartialView("_VideoList", videos);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadVideo(int postId, IFormFile file)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(item => item.Id == postId);
            if (post == null) return NotFound();
            if (!CanManagePost(post)) return Forbid();

            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "Odaberite video datoteku." });
            }

            if (!IsAllowedVideo(file))
            {
                return BadRequest(new { message = "Dopusteni su samo video zapisi do 100 MB." });
            }

            await SavePostVideo(postId, file);

            return Json(new { success = true });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteVideo(int id)
        {
            var video = await _context.PostVideos
                .Include(item => item.Post)
                .FirstOrDefaultAsync(item => item.Id == id);
            if (video == null) return NotFound();
            if (!CanManagePost(video.Post)) return Forbid();

            DeletePhysicalFile(video.FilePath);

            _context.PostVideos.Remove(video);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [Authorize]
        public async Task<IActionResult> Create()
        {
            await PopulateTags();
            return View(new PostFormModel
            {
                CreatedAt = DateTime.Now
            });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PostFormModel model, IFormFile? videoFile)
        {
            model.AuthorId = GetCurrentUserId();
            await ValidatePostForm(model);

            if (videoFile != null && videoFile.Length > 0 && !IsAllowedVideo(videoFile))
            {
                ModelState.AddModelError(nameof(videoFile), "Dopusteni su samo MP4, WebM, MOV ili M4V videozapisi do 100 MB.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateTags();
                return View(model);
            }

            var post = new Post();
            await MapToEntity(model, post);

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            if (videoFile != null && videoFile.Length > 0)
            {
                await SavePostVideo(post.Id, videoFile);
            }

            TempData["Flash"] = "Post created successfully.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var post = await _context.Posts
                .Include(item => item.Author)
                .Include(item => item.Tags)
                .FirstOrDefaultAsync(item => item.Id == id);

            if (post == null) return NotFound();
            if (!CanManagePost(post)) return Forbid();

            await PopulateTags();
            return View(MapToForm(post));
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PostFormModel model)
        {
            if (id != model.Id) return NotFound();

            var post = await _context.Posts
                .Include(item => item.Tags)
                .FirstOrDefaultAsync(item => item.Id == id);

            if (post == null) return NotFound();
            if (!CanManagePost(post)) return Forbid();

            await ValidatePostForm(model);

            if (!ModelState.IsValid)
            {
                await PopulateTags();
                return View(model);
            }

            await MapToEntity(model, post);
            await _context.SaveChangesAsync();

            TempData["Flash"] = "Post updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var post = await _context.Posts
                .Include(item => item.Author)
                .FirstOrDefaultAsync(item => item.Id == id);

            if (post == null) return NotFound();
            if (!CanManagePost(post)) return Forbid();
            return View(post);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts
                .Include(item => item.Videos)
                .FirstOrDefaultAsync(item => item.Id == id);

            if (post == null) return NotFound();
            if (!CanManagePost(post)) return Forbid();

            foreach (var video in post.Videos)
            {
                DeletePhysicalFile(video.FilePath);
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            TempData["Flash"] = "Post deleted.";
            return RedirectToAction(nameof(Index));
        }

        private static bool IsAllowedVideo(IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var allowedExtensions = new[] { ".mp4", ".webm", ".mov", ".m4v" };
            const long maxSize = 100 * 1024 * 1024;

            return file.Length <= maxSize &&
                   file.ContentType.StartsWith("video/", StringComparison.OrdinalIgnoreCase) &&
                   allowedExtensions.Contains(extension);
        }

        private async Task SavePostVideo(int postId, IFormFile file)
        {
            var relativeDirectory = Path.Combine("uploads", "posts", "videos");
            var uploadsPath = Path.Combine(_environment.WebRootPath, relativeDirectory);
            Directory.CreateDirectory(uploadsPath);

            var storedFileName = Guid.NewGuid().ToString("N") + Path.GetExtension(file.FileName);
            var physicalPath = Path.Combine(uploadsPath, storedFileName);

            await using (var stream = new FileStream(physicalPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var video = new PostVideo
            {
                PostId = postId,
                FileName = Path.GetFileName(file.FileName),
                FilePath = "/" + relativeDirectory.Replace(Path.DirectorySeparatorChar, '/') + "/" + storedFileName,
                ContentType = file.ContentType,
                FileSize = file.Length,
                CreatedAt = DateTime.UtcNow
            };

            _context.PostVideos.Add(video);
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

        private IQueryable<Post> BuildQuery(string? category, string? q)
        {
            q = q?.Trim();

            var postsQuery = _context.Posts
                .Include(post => post.Author)
                    .ThenInclude(author => author.ProfileImages)
                .Include(post => post.Comments)
                .Include(post => post.Tags)
                .Include(post => post.Likes)
                .AsQueryable();

            if (!string.IsNullOrEmpty(category) &&
                Enum.TryParse<PostCategory>(category, out var cat))
            {
                postsQuery = postsQuery.Where(post => post.Category == cat);
            }

            if (!string.IsNullOrEmpty(q))
            {
                postsQuery = postsQuery.Where(post =>
                    EF.Functions.Like(post.Title, $"%{q}%") ||
                    EF.Functions.Like(post.Content, $"%{q}%") ||
                    EF.Functions.Like(post.Author.UserName, $"%{q}%") ||
                    post.Tags.Any(tag => EF.Functions.Like(tag.Name, $"%{q}%")));
            }

            return postsQuery.OrderByDescending(post => post.CreatedAt);
        }

        private async Task PopulateTags()
        {
            ViewBag.Tags = await _context.Tags
                .OrderBy(tag => tag.Name)
                .ToListAsync();
        }

        private async Task ValidatePostForm(PostFormModel model)
        {
            if (model.AuthorId.HasValue &&
                !await _context.Users.AnyAsync(user => user.Id == model.AuthorId.Value))
            {
                ModelState.AddModelError(nameof(model.AuthorId), "Selected author does not exist.");
            }
        }

        private PostFormModel MapToForm(Post post)
        {
            return new PostFormModel
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                Category = post.Category,
                CreatedAt = post.CreatedAt,
                AuthorId = post.AuthorId,
                AuthorText = post.Author == null ? null : "@" + post.Author.UserName,
                TagIds = post.Tags.Select(tag => tag.Id).ToList()
            };
        }

        private async Task MapToEntity(PostFormModel model, Post post)
        {
            post.Title = model.Title.Trim();
            post.Content = model.Content.Trim();
            post.Category = model.Category!.Value;
            post.CreatedAt = model.CreatedAt!.Value;
            if (model.AuthorId.HasValue)
            {
                post.AuthorId = model.AuthorId.Value;
            }

            post.Tags.Clear();
            var tags = await _context.Tags
                .Where(tag => model.TagIds.Contains(tag.Id))
                .ToListAsync();

            foreach (var tag in tags)
            {
                post.Tags.Add(tag);
            }
        }

        private int GetCurrentUserId()
        {
            var userId = _userManager.GetUserId(User);
            if (!int.TryParse(userId, out var parsedUserId))
            {
                throw new InvalidOperationException("Prijavljeni korisnik nema ispravan identifikator.");
            }

            return parsedUserId;
        }

        private bool CanManagePost(Post post)
        {
            var currentUserId = _userManager.GetUserId(User);
            return User.IsInRole("Admin") || post.AuthorId.ToString() == currentUserId;
        }

        private async Task AddNotification(int recipientId, int actorId, string message)
        {
            if (recipientId == actorId) return;

            _context.Notifications.Add(new Notification
            {
                RecipientId = recipientId,
                Message = message,
                CreatedAt = DateTime.Now
            });

            await Task.CompletedTask;
        }

        private string GetDisplayName()
        {
            return User.Identity?.Name ?? "Someone";
        }
    }
}
