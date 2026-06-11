using Filip_Rados_lab5.Data;
using Filip_Rados_lab5.Models;
using Filip_Rados_lab5.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Filip_Rados_lab5.Controllers
{
    [Authorize(Roles = "Admin")]
    public class FollowsController : Controller
    {
        private readonly AppDbContext _context;

        public FollowsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? q)
        {
            var follows = await BuildQuery(q).ToListAsync();
            ViewBag.Query = q;
            return View(follows);
        }

        public async Task<IActionResult> Search(string? q)
        {
            var follows = await BuildQuery(q).ToListAsync();
            return PartialView("_List", follows);
        }

        public async Task<IActionResult> Details(int id)
        {
            var follow = await _context.Follows
                .Include(item => item.Follower)
                .Include(item => item.Following)
                .FirstOrDefaultAsync(item => item.Id == id);

            if (follow == null) return NotFound();
            return View(follow);
        }

        public IActionResult Create()
        {
            return View(new FollowFormModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FollowFormModel model)
        {
            await ValidateFollowForm(model);

            if (!ModelState.IsValid) return View(model);

            _context.Follows.Add(new Follow
            {
                FollowerId = model.FollowerId!.Value,
                FollowingId = model.FollowingId!.Value
            });

            await _context.SaveChangesAsync();
            TempData["Flash"] = "Pracenje je uspjesno kreirano.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var follow = await _context.Follows
                .Include(item => item.Follower)
                .Include(item => item.Following)
                .FirstOrDefaultAsync(item => item.Id == id);

            if (follow == null) return NotFound();
            return View(MapToForm(follow));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FollowFormModel model)
        {
            if (id != model.Id) return NotFound();

            var follow = await _context.Follows.FindAsync(id);
            if (follow == null) return NotFound();

            await ValidateFollowForm(model);

            if (!ModelState.IsValid) return View(model);

            follow.FollowerId = model.FollowerId!.Value;
            follow.FollowingId = model.FollowingId!.Value;

            await _context.SaveChangesAsync();
            TempData["Flash"] = "Pracenje je uspjesno azurirano.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var follow = await _context.Follows
                .Include(item => item.Follower)
                .Include(item => item.Following)
                .FirstOrDefaultAsync(item => item.Id == id);

            if (follow == null) return NotFound();
            return View(follow);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var follow = await _context.Follows.FindAsync(id);
            if (follow == null) return NotFound();

            _context.Follows.Remove(follow);
            await _context.SaveChangesAsync();

            TempData["Flash"] = "Pracenje je obrisano.";
            return RedirectToAction(nameof(Index));
        }

        private IQueryable<Follow> BuildQuery(string? q)
        {
            q = q?.Trim();

            var query = _context.Follows
                .Include(follow => follow.Follower)
                .Include(follow => follow.Following)
                .AsQueryable();

            if (!string.IsNullOrEmpty(q))
            {
                query = query.Where(follow =>
                    EF.Functions.Like(follow.Follower.UserName, $"%{q}%") ||
                    EF.Functions.Like(follow.Following.UserName, $"%{q}%"));
            }

            return query.OrderBy(follow => follow.Follower.UserName).ThenBy(follow => follow.Following.UserName);
        }

        private async Task ValidateFollowForm(FollowFormModel model)
        {
            if (model.FollowerId.HasValue &&
                !await _context.Users.AnyAsync(user => user.Id == model.FollowerId.Value))
            {
                ModelState.AddModelError(nameof(model.FollowerId), "Korisnik koji prati ne postoji.");
            }

            if (model.FollowingId.HasValue &&
                !await _context.Users.AnyAsync(user => user.Id == model.FollowingId.Value))
            {
                ModelState.AddModelError(nameof(model.FollowingId), "Korisnik koji se prati ne postoji.");
            }

            if (model.FollowerId.HasValue &&
                model.FollowingId.HasValue &&
                model.FollowerId.Value == model.FollowingId.Value)
            {
                ModelState.AddModelError(nameof(model.FollowingId), "Korisnik ne moze pratiti sam sebe.");
            }

            if (model.FollowerId.HasValue && model.FollowingId.HasValue &&
                await _context.Follows.AnyAsync(follow =>
                    follow.Id != model.Id &&
                    follow.FollowerId == model.FollowerId.Value &&
                    follow.FollowingId == model.FollowingId.Value))
            {
                ModelState.AddModelError(nameof(model.FollowingId), "Takvo pracenje vec postoji.");
            }
        }

        private static FollowFormModel MapToForm(Follow follow)
        {
            return new FollowFormModel
            {
                Id = follow.Id,
                FollowerId = follow.FollowerId,
                FollowerText = follow.Follower == null ? null : "@" + follow.Follower.UserName,
                FollowingId = follow.FollowingId,
                FollowingText = follow.Following == null ? null : "@" + follow.Following.UserName
            };
        }
    }
}
