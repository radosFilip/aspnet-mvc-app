using Filip_Rados_lab5.Data;
using Filip_Rados_lab5.Models;
using Filip_Rados_lab5.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Filip_Rados_lab5.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CommentsController : Controller
    {
        private readonly AppDbContext _context;

        public CommentsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? q)
        {
            var comments = await BuildQuery(q).ToListAsync();
            ViewBag.Query = q;
            return View(comments);
        }

        public async Task<IActionResult> Search(string? q)
        {
            var comments = await BuildQuery(q).ToListAsync();
            return PartialView("_List", comments);
        }

        public async Task<IActionResult> Details(int id)
        {
            var comment = await _context.Comments
                .Include(item => item.Author)
                .Include(item => item.Post)
                .FirstOrDefaultAsync(item => item.Id == id);

            if (comment == null) return NotFound();
            return View(comment);
        }

        public IActionResult Create()
        {
            return View(new CommentFormModel
            {
                CreatedAt = DateTime.Now
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CommentFormModel model)
        {
            await ValidateCommentForm(model);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var comment = new Comment();
            MapToEntity(model, comment);

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            TempData["Flash"] = "Komentar je uspjesno kreiran.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var comment = await _context.Comments
                .Include(item => item.Author)
                .Include(item => item.Post)
                .FirstOrDefaultAsync(item => item.Id == id);

            if (comment == null) return NotFound();
            return View(MapToForm(comment));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CommentFormModel model)
        {
            if (id != model.Id) return NotFound();

            var comment = await _context.Comments.FindAsync(id);
            if (comment == null) return NotFound();

            await ValidateCommentForm(model);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            MapToEntity(model, comment);
            await _context.SaveChangesAsync();

            TempData["Flash"] = "Komentar je uspjesno azuriran.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var comment = await _context.Comments
                .Include(item => item.Author)
                .Include(item => item.Post)
                .FirstOrDefaultAsync(item => item.Id == id);

            if (comment == null) return NotFound();
            return View(comment);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null) return NotFound();

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            TempData["Flash"] = "Komentar je obrisan.";
            return RedirectToAction(nameof(Index));
        }

        private IQueryable<Comment> BuildQuery(string? q)
        {
            q = q?.Trim();

            var query = _context.Comments
                .Include(comment => comment.Author)
                .Include(comment => comment.Post)
                .AsQueryable();

            if (!string.IsNullOrEmpty(q))
            {
                query = query.Where(comment =>
                    EF.Functions.Like(comment.Content, $"%{q}%") ||
                    EF.Functions.Like(comment.Author.UserName, $"%{q}%") ||
                    EF.Functions.Like(comment.Post.Title, $"%{q}%"));
            }

            return query.OrderByDescending(comment => comment.CreatedAt);
        }

        private async Task ValidateCommentForm(CommentFormModel model)
        {
            if (model.AuthorId.HasValue &&
                !await _context.Users.AnyAsync(user => user.Id == model.AuthorId.Value))
            {
                ModelState.AddModelError(nameof(model.AuthorId), "Odabrani autor ne postoji.");
            }

            if (model.PostId.HasValue &&
                !await _context.Posts.AnyAsync(post => post.Id == model.PostId.Value))
            {
                ModelState.AddModelError(nameof(model.PostId), "Odabrani post ne postoji.");
            }
        }

        private static CommentFormModel MapToForm(Comment comment)
        {
            return new CommentFormModel
            {
                Id = comment.Id,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                AuthorId = comment.AuthorId,
                AuthorText = comment.Author == null ? null : "@" + comment.Author.UserName,
                PostId = comment.PostId,
                PostText = comment.Post?.Title
            };
        }

        private static void MapToEntity(CommentFormModel model, Comment comment)
        {
            comment.Content = model.Content.Trim();
            comment.CreatedAt = model.CreatedAt!.Value;
            comment.AuthorId = model.AuthorId!.Value;
            comment.PostId = model.PostId!.Value;
        }
    }
}
