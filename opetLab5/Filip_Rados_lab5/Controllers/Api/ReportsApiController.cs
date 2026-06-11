using Filip_Rados_lab5.Data;
using Filip_Rados_lab5.Dtos;
using Filip_Rados_lab5.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Filip_Rados_lab5.Controllers.Api
{
    [Route("api/reports")]
    [ApiController]
    public class ReportsApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReportsApiController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReportDto>>> Get(string? q)
        {
            q = q?.Trim();

            var query = BuildReportQuery();

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

            var reports = await query
                .OrderByDescending(report => report.CreatedAt)
                .ToListAsync();

            return Ok(reports.Select(report => report.ToDto()));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ReportDto>> Get(int id)
        {
            var report = await BuildReportQuery()
                .FirstOrDefaultAsync(item => item.Id == id);

            if (report == null) return NotFound();

            return Ok(report.ToDto());
        }

        [HttpPost]
        public async Task<ActionResult<ReportDto>> Post(ReportCreateUpdateDto dto)
        {
            await ValidateReport(dto);
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var report = new Report();
            MapToEntity(dto, report);

            _context.Reports.Add(report);
            await _context.SaveChangesAsync();

            var createdReport = await BuildReportQuery().FirstAsync(item => item.Id == report.Id);
            return CreatedAtAction(nameof(Get), new { id = report.Id }, createdReport.ToDto());
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ReportDto>> Put(int id, ReportCreateUpdateDto dto)
        {
            var report = await _context.Reports.FindAsync(id);
            if (report == null) return NotFound();

            await ValidateReport(dto);
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            MapToEntity(dto, report);
            await _context.SaveChangesAsync();

            var updatedReport = await BuildReportQuery().FirstAsync(item => item.Id == id);
            return Ok(updatedReport.ToDto());
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var report = await _context.Reports.FindAsync(id);
            if (report == null) return NotFound();

            _context.Reports.Remove(report);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private IQueryable<Report> BuildReportQuery()
        {
            return _context.Reports
                .Include(report => report.Reporter)
                .Include(report => report.Post);
        }

        private async Task ValidateReport(ReportCreateUpdateDto dto)
        {
            if (dto.ReporterId.HasValue &&
                !await _context.Users.AnyAsync(user => user.Id == dto.ReporterId.Value))
            {
                ModelState.AddModelError(nameof(dto.ReporterId), "Odabrani prijavitelj ne postoji.");
            }

            if (dto.PostId.HasValue &&
                !await _context.Posts.AnyAsync(post => post.Id == dto.PostId.Value))
            {
                ModelState.AddModelError(nameof(dto.PostId), "Odabrani post ne postoji.");
            }
        }

        private static void MapToEntity(ReportCreateUpdateDto dto, Report report)
        {
            report.Reason = dto.Reason!.Value;
            report.Status = dto.Status!.Value;
            report.CreatedAt = dto.CreatedAt ?? DateTime.Now;
            report.ReporterId = dto.ReporterId!.Value;
            report.PostId = dto.PostId!.Value;
        }
    }
}
