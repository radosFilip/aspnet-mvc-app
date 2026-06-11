using Filip_Rados_lab5.Data;
using Filip_Rados_lab5.Models;
using Filip_Rados_lab5.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Filip_Rados_lab5.Controllers
{
    public class TagsController : Controller
    {
        private readonly AppDbContext _context;

        public TagsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? q)
        {
            var tags = await BuildQuery(q).ToListAsync();
            ViewBag.Query = q;
            return View(tags);
        }

        public async Task<IActionResult> Search(string? q)
        {
            var tags = await BuildQuery(q).ToListAsync();
            return PartialView("_List", tags);
        }

        public async Task<IActionResult> Autocomplete(string? term)
        {
            term = term?.Trim();

            var tags = await _context.Tags
                .Where(tag => string.IsNullOrEmpty(term) || EF.Functions.Like(tag.Name, $"%{term}%"))
                .OrderBy(tag => tag.Name)
                .Take(12)
                .Select(tag => new
                {
                    id = tag.Id,
                    text = "#" + tag.Name,
                    subtitle = tag.Posts.Count + " objava"
                })
                .ToListAsync();

            return Json(tags);
        }

        public async Task<IActionResult> Details(int id)
        {
            var tag = await _context.Tags.FirstOrDefaultAsync(item => item.Id == id);
            if (tag == null) return NotFound();

            ViewBag.Posts = await _context.Posts
                .Include(post => post.Author)
                .Include(post => post.Comments)
                .Include(post => post.Tags)
                .Include(post => post.Likes)
                .Where(post => post.Tags.Any(item => item.Id == id))
                .OrderByDescending(post => post.CreatedAt)
                .ToListAsync();

            return View(tag);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View(new TagFormModel());
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TagFormModel model)
        {
            await ValidateUniqueTag(model);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            _context.Tags.Add(new Tag { Name = model.Name.Trim() });
            await _context.SaveChangesAsync();

            TempData["Flash"] = "Tag je uspjesno kreiran.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var tag = await _context.Tags.FindAsync(id);
            if (tag == null) return NotFound();

            return View(new TagFormModel { Id = tag.Id, Name = tag.Name });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TagFormModel model)
        {
            if (id != model.Id) return NotFound();

            var tag = await _context.Tags.FindAsync(id);
            if (tag == null) return NotFound();

            await ValidateUniqueTag(model);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            tag.Name = model.Name.Trim();
            await _context.SaveChangesAsync();

            TempData["Flash"] = "Tag je uspjesno azuriran.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var tag = await _context.Tags
                .Include(item => item.Posts)
                .FirstOrDefaultAsync(item => item.Id == id);

            if (tag == null) return NotFound();

            ViewBag.DeleteBlockedReason = tag.Posts.Any()
                ? "Tag je povezan s objavama pa ga nije moguce obrisati."
                : null;

            return View(tag);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tag = await _context.Tags
                .Include(item => item.Posts)
                .FirstOrDefaultAsync(item => item.Id == id);

            if (tag == null) return NotFound();

            if (tag.Posts.Any())
            {
                var reason = "Tag je povezan s objavama pa ga nije moguce obrisati.";
                ModelState.AddModelError(string.Empty, reason);
                ViewBag.DeleteBlockedReason = reason;
                return View(tag);
            }

            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();

            TempData["Flash"] = "Tag je obrisan.";
            return RedirectToAction(nameof(Index));
        }

        private IQueryable<Tag> BuildQuery(string? q)
        {
            q = q?.Trim();

            var query = _context.Tags
                .Include(tag => tag.Posts)
                .AsQueryable();

            if (!string.IsNullOrEmpty(q))
            {
                query = query.Where(tag => EF.Functions.Like(tag.Name, $"%{q}%"));
            }

            return query.OrderBy(tag => tag.Name);
        }

        private async Task ValidateUniqueTag(TagFormModel model)
        {
            if (await _context.Tags.AnyAsync(tag => tag.Id != model.Id && tag.Name == model.Name))
            {
                ModelState.AddModelError(nameof(model.Name), "Tag s tim nazivom vec postoji.");
            }
        }
    }
}
