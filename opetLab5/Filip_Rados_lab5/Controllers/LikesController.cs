using Filip_Rados_lab5.Data;
using Filip_Rados_lab5.Models;
using Filip_Rados_lab5.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Filip_Rados_lab5.Controllers
{
    [Authorize(Roles = "Admin")]
    public class LikesController : Controller
    {
        private readonly AppDbContext _context;

        public LikesController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? q)
        {
            var likes = await BuildQuery(q).ToListAsync();
            ViewBag.Query = q;
            return View(likes);
        }

        public async Task<IActionResult> Search(string? q)
        {
            var likes = await BuildQuery(q).ToListAsync();
            return PartialView("_List", likes);
        }

        public async Task<IActionResult> Details(int id)
        {
            var like = await _context.Likes
                .Include(item => item.User)
                .Include(item => item.Post)
                .FirstOrDefaultAsync(item => item.Id == id);

            if (like == null) return NotFound();
            return View(like);
        }

        public IActionResult Create()
        {
            return View(new LikeFormModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LikeFormModel model)
        {
            await ValidateLikeForm(model);

            if (!ModelState.IsValid) return View(model);

            _context.Likes.Add(new Like
            {
                UserId = model.UserId!.Value,
                PostId = model.PostId!.Value
            });

            await _context.SaveChangesAsync();
            TempData["Flash"] = "Lajk je uspjesno kreiran.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var like = await _context.Likes
                .Include(item => item.User)
                .Include(item => item.Post)
                .FirstOrDefaultAsync(item => item.Id == id);

            if (like == null) return NotFound();
            return View(MapToForm(like));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LikeFormModel model)
        {
            if (id != model.Id) return NotFound();

            var like = await _context.Likes.FindAsync(id);
            if (like == null) return NotFound();

            await ValidateLikeForm(model);

            if (!ModelState.IsValid) return View(model);

            like.UserId = model.UserId!.Value;
            like.PostId = model.PostId!.Value;

            await _context.SaveChangesAsync();
            TempData["Flash"] = "Lajk je uspjesno azuriran.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var like = await _context.Likes
                .Include(item => item.User)
                .Include(item => item.Post)
                .FirstOrDefaultAsync(item => item.Id == id);

            if (like == null) return NotFound();
            return View(like);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var like = await _context.Likes.FindAsync(id);
            if (like == null) return NotFound();

            _context.Likes.Remove(like);
            await _context.SaveChangesAsync();

            TempData["Flash"] = "Lajk je obrisan.";
            return RedirectToAction(nameof(Index));
        }

        private IQueryable<Like> BuildQuery(string? q)
        {
            q = q?.Trim();

            var query = _context.Likes
                .Include(like => like.User)
                .Include(like => like.Post)
                .AsQueryable();

            if (!string.IsNullOrEmpty(q))
            {
                query = query.Where(like =>
                    EF.Functions.Like(like.User.UserName, $"%{q}%") ||
                    EF.Functions.Like(like.Post.Title, $"%{q}%"));
            }

            return query.OrderBy(like => like.User.UserName).ThenBy(like => like.Post.Title);
        }

        private async Task ValidateLikeForm(LikeFormModel model)
        {
            if (model.UserId.HasValue &&
                !await _context.Users.AnyAsync(user => user.Id == model.UserId.Value))
            {
                ModelState.AddModelError(nameof(model.UserId), "Odabrani korisnik ne postoji.");
            }

            if (model.PostId.HasValue &&
                !await _context.Posts.AnyAsync(post => post.Id == model.PostId.Value))
            {
                ModelState.AddModelError(nameof(model.PostId), "Odabrani post ne postoji.");
            }

            if (model.UserId.HasValue && model.PostId.HasValue &&
                await _context.Likes.AnyAsync(like =>
                    like.Id != model.Id &&
                    like.UserId == model.UserId.Value &&
                    like.PostId == model.PostId.Value))
            {
                ModelState.AddModelError(nameof(model.PostId), "Taj korisnik je vec lajkao odabrani post.");
            }
        }

        private static LikeFormModel MapToForm(Like like)
        {
            return new LikeFormModel
            {
                Id = like.Id,
                UserId = like.UserId,
                UserText = like.User == null ? null : "@" + like.User.UserName,
                PostId = like.PostId,
                PostText = like.Post?.Title
            };
        }
    }
}
