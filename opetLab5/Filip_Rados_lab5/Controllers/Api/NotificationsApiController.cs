using Filip_Rados_lab5.Data;
using Filip_Rados_lab5.Dtos;
using Filip_Rados_lab5.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Filip_Rados_lab5.Controllers.Api
{
    [Route("api/notifications")]
    [ApiController]
    public class NotificationsApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public NotificationsApiController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> Get(string? q)
        {
            q = q?.Trim();

            var query = BuildNotificationQuery();

            if (!string.IsNullOrEmpty(q))
            {
                query = query.Where(notification =>
                    EF.Functions.Like(notification.Message, $"%{q}%") ||
                    EF.Functions.Like(notification.Recipient.UserName, $"%{q}%"));
            }

            var notifications = await query
                .OrderByDescending(notification => notification.CreatedAt)
                .ToListAsync();

            return Ok(notifications.Select(notification => notification.ToDto()));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<NotificationDto>> Get(int id)
        {
            var notification = await BuildNotificationQuery()
                .FirstOrDefaultAsync(item => item.Id == id);

            if (notification == null) return NotFound();

            return Ok(notification.ToDto());
        }

        [HttpPost]
        public async Task<ActionResult<NotificationDto>> Post(NotificationCreateUpdateDto dto)
        {
            await ValidateNotification(dto);
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var notification = new Notification();
            MapToEntity(dto, notification);

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            var createdNotification = await BuildNotificationQuery().FirstAsync(item => item.Id == notification.Id);
            return CreatedAtAction(nameof(Get), new { id = notification.Id }, createdNotification.ToDto());
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<NotificationDto>> Put(int id, NotificationCreateUpdateDto dto)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null) return NotFound();

            await ValidateNotification(dto);
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            MapToEntity(dto, notification);
            await _context.SaveChangesAsync();

            var updatedNotification = await BuildNotificationQuery().FirstAsync(item => item.Id == id);
            return Ok(updatedNotification.ToDto());
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null) return NotFound();

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private IQueryable<Notification> BuildNotificationQuery()
        {
            return _context.Notifications
                .Include(notification => notification.Recipient);
        }

        private async Task ValidateNotification(NotificationCreateUpdateDto dto)
        {
            if (dto.RecipientId.HasValue &&
                !await _context.Users.AnyAsync(user => user.Id == dto.RecipientId.Value))
            {
                ModelState.AddModelError(nameof(dto.RecipientId), "Odabrani primatelj ne postoji.");
            }
        }

        private static void MapToEntity(NotificationCreateUpdateDto dto, Notification notification)
        {
            notification.Message = dto.Message.Trim();
            notification.CreatedAt = dto.CreatedAt ?? DateTime.Now;
            notification.RecipientId = dto.RecipientId!.Value;
        }
    }
}
