using Filip_Rados_lab5.Data;
using Filip_Rados_lab5.Models;
using Filip_Rados_lab5.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Filip_Rados_lab5.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReportsController : Controller
    {
        private readonly AppDbContext _context;

        public ReportsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? q)
        {
            var reports = await BuildQuery(q).ToListAsync();
            ViewBag.Query = q;
            return View(reports);
        }

        public async Task<IActionResult> Search(string? q)
        {
            var reports = await BuildQuery(q).ToListAsync();
            return PartialView("_List", reports);
        }

        public async Task<IActionResult> Details(int id)
        {
            var report = await _context.Reports
                .Include(item => item.Reporter)
                .Include(item => item.Post)
                .FirstOrDefaultAsync(item => item.Id == id);

            if (report == null) return NotFound();
            return View(report);
        }

        public IActionResult Create()
        {
            return View(new ReportFormModel
            {
                CreatedAt = DateTime.Now,
                Status = ReportStatus.Pending
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReportFormModel model)
        {
            await ValidateReportForm(model);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var report = new Report();
            MapToEntity(model, report);

            _context.Reports.Add(report);
            await _context.SaveChangesAsync();

            TempData["Flash"] = "Prijava je uspjesno kreirana.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var report = await _context.Reports
                .Include(item => item.Reporter)
                .Include(item => item.Post)
                .FirstOrDefaultAsync(item => item.Id == id);

            if (report == null) return NotFound();
            return View(MapToForm(report));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ReportFormModel model)
        {
            if (id != model.Id) return NotFound();

            var report = await _context.Reports.FindAsync(id);
            if (report == null) return NotFound();

            await ValidateReportForm(model);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            MapToEntity(model, report);
            await _context.SaveChangesAsync();

            TempData["Flash"] = "Prijava je uspjesno azurirana.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var report = await _context.Reports
                .Include(item => item.Reporter)
                .Include(item => item.Post)
                .FirstOrDefaultAsync(item => item.Id == id);

            if (report == null) return NotFound();
            return View(report);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var report = await _context.Reports.FindAsync(id);
            if (report == null) return NotFound();

            _context.Reports.Remove(report);
            await _context.SaveChangesAsync();

            TempData["Flash"] = "Prijava je obrisana.";
            return RedirectToAction(nameof(Index));
        }

        private IQueryable<Report> BuildQuery(string? q)
        {
            q = q?.Trim();

            var query = _context.Reports
                .Include(report => report.Reporter)
                .Include(report => report.Post)
                .AsQueryable();

            if (!string.IsNullOrEmpty(q))
            {
                var matchingReasons = Enum.GetValues<ReportReason>()
                    .Where(reason => reason.ToString().Contains(q, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                var matchingStatuses = Enum.GetValues<ReportStatus>()
                    .Where(status => status.ToString().Contains(q, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                query = query.Where(report =>
                    EF.Functions.Like(report.Reporter.UserName, $"%{q}%") ||
                    EF.Functions.Like(report.Post.Title, $"%{q}%") ||
                    matchingReasons.Contains(report.Reason) ||
                    matchingStatuses.Contains(report.Status));
            }

            return query.OrderByDescending(report => report.CreatedAt);
        }

        private async Task ValidateReportForm(ReportFormModel model)
        {
            if (model.ReporterId.HasValue &&
                !await _context.Users.AnyAsync(user => user.Id == model.ReporterId.Value))
            {
                ModelState.AddModelError(nameof(model.ReporterId), "Odabrani prijavitelj ne postoji.");
            }

            if (model.PostId.HasValue &&
                !await _context.Posts.AnyAsync(post => post.Id == model.PostId.Value))
            {
                ModelState.AddModelError(nameof(model.PostId), "Odabrani post ne postoji.");
            }
        }

        private static ReportFormModel MapToForm(Report report)
        {
            return new ReportFormModel
            {
                Id = report.Id,
                Reason = report.Reason,
                Status = report.Status,
                CreatedAt = report.CreatedAt,
                ReporterId = report.ReporterId,
                ReporterText = report.Reporter == null ? null : "@" + report.Reporter.UserName,
                PostId = report.PostId,
                PostText = report.Post?.Title
            };
        }

        private static void MapToEntity(ReportFormModel model, Report report)
        {
            report.Reason = model.Reason!.Value;
            report.Status = model.Status!.Value;
            report.CreatedAt = model.CreatedAt!.Value;
            report.ReporterId = model.ReporterId!.Value;
            report.PostId = model.PostId!.Value;
        }
    }
}
