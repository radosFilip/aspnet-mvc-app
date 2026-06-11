using Filip_Rados_lab5.Data;
using Filip_Rados_lab5.Models;
using Filip_Rados_lab5.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Filip_Rados_lab5.Controllers
{
    [Authorize]
    public class NotificationsController : Controller
    {
        private readonly AppDbContext _context;

        public NotificationsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? q)
        {
            var notifications = await BuildQuery(q).ToListAsync();
            ViewBag.Query = q;
            return View(notifications);
        }

        public async Task<IActionResult> Search(string? q)
        {
            var notifications = await BuildQuery(q).ToListAsync();
            return PartialView("_List", notifications);
        }

        public async Task<IActionResult> Details(int id)
        {
            var notification = await _context.Notifications
                .Include(item => item.Recipient)
                .FirstOrDefaultAsync(item => item.Id == id);

            if (notification == null) return NotFound();
            if (!CanViewNotification(notification)) return Forbid();

            return View(notification);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View(new NotificationFormModel
            {
                CreatedAt = DateTime.Now
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NotificationFormModel model)
        {
            await ValidateNotificationForm(model);

            if (!ModelState.IsValid) return View(model);

            _context.Notifications.Add(new Notification
            {
                Message = model.Message.Trim(),
                CreatedAt = model.CreatedAt!.Value,
                RecipientId = model.RecipientId!.Value
            });

            await _context.SaveChangesAsync();
            TempData["Flash"] = "Notification created successfully.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var notification = await _context.Notifications
                .Include(item => item.Recipient)
                .FirstOrDefaultAsync(item => item.Id == id);

            if (notification == null) return NotFound();
            return View(MapToForm(notification));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, NotificationFormModel model)
        {
            if (id != model.Id) return NotFound();

            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null) return NotFound();

            await ValidateNotificationForm(model);

            if (!ModelState.IsValid) return View(model);

            notification.Message = model.Message.Trim();
            notification.CreatedAt = model.CreatedAt!.Value;
            notification.RecipientId = model.RecipientId!.Value;

            await _context.SaveChangesAsync();
            TempData["Flash"] = "Notification updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var notification = await _context.Notifications
                .Include(item => item.Recipient)
                .FirstOrDefaultAsync(item => item.Id == id);

            if (notification == null) return NotFound();
            return View(notification);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null) return NotFound();

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();

            TempData["Flash"] = "Notification deleted.";
            return RedirectToAction(nameof(Index));
        }

        private IQueryable<Notification> BuildQuery(string? q)
        {
            q = q?.Trim();

            var query = _context.Notifications
                .Include(notification => notification.Recipient)
                .AsQueryable();

            if (!User.IsInRole("Admin"))
            {
                var currentUserId = GetCurrentUserId();
                query = query.Where(notification => notification.RecipientId == currentUserId);
            }

            if (!string.IsNullOrEmpty(q))
            {
                query = query.Where(notification =>
                    EF.Functions.Like(notification.Message, $"%{q}%") ||
                    EF.Functions.Like(notification.Recipient.UserName, $"%{q}%"));
            }

            return query.OrderByDescending(notification => notification.CreatedAt);
        }

        private async Task ValidateNotificationForm(NotificationFormModel model)
        {
            if (model.RecipientId.HasValue &&
                !await _context.Users.AnyAsync(user => user.Id == model.RecipientId.Value))
            {
                ModelState.AddModelError(nameof(model.RecipientId), "Selected recipient does not exist.");
            }
        }

        private static NotificationFormModel MapToForm(Notification notification)
        {
            return new NotificationFormModel
            {
                Id = notification.Id,
                Message = notification.Message,
                CreatedAt = notification.CreatedAt,
                RecipientId = notification.RecipientId,
                RecipientText = notification.Recipient == null ? null : "@" + notification.Recipient.UserName
            };
        }

        private int GetCurrentUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(userId, out var parsedUserId) ? parsedUserId : 0;
        }

        private bool CanViewNotification(Notification notification)
        {
            return User.IsInRole("Admin") || notification.RecipientId == GetCurrentUserId();
        }
    }
}
