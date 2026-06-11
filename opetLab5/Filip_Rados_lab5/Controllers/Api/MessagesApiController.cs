using Filip_Rados_lab5.Data;
using Filip_Rados_lab5.Dtos;
using Filip_Rados_lab5.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Filip_Rados_lab5.Controllers.Api
{
    [Route("api/messages")]
    [ApiController]
    public class MessagesApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MessagesApiController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDto>>> Get(string? q)
        {
            q = q?.Trim();

            var query = BuildMessageQuery();

            if (!string.IsNullOrEmpty(q))
            {
                query = query.Where(message =>
                    EF.Functions.Like(message.Content, $"%{q}%") ||
                    EF.Functions.Like(message.Sender.UserName, $"%{q}%") ||
                    EF.Functions.Like(message.Receiver.UserName, $"%{q}%"));
            }

            var messages = await query
                .OrderByDescending(message => message.SentAt)
                .ToListAsync();

            return Ok(messages.Select(message => message.ToDto()));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<MessageDto>> Get(int id)
        {
            var message = await BuildMessageQuery()
                .FirstOrDefaultAsync(item => item.Id == id);

            if (message == null) return NotFound();

            return Ok(message.ToDto());
        }

        [HttpPost]
        public async Task<ActionResult<MessageDto>> Post(MessageCreateUpdateDto dto)
        {
            await ValidateMessage(dto);
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var message = new Message();
            MapToEntity(dto, message);

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            var createdMessage = await BuildMessageQuery().FirstAsync(item => item.Id == message.Id);
            return CreatedAtAction(nameof(Get), new { id = message.Id }, createdMessage.ToDto());
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<MessageDto>> Put(int id, MessageCreateUpdateDto dto)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message == null) return NotFound();

            await ValidateMessage(dto);
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            MapToEntity(dto, message);
            await _context.SaveChangesAsync();

            var updatedMessage = await BuildMessageQuery().FirstAsync(item => item.Id == id);
            return Ok(updatedMessage.ToDto());
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message == null) return NotFound();

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private IQueryable<Message> BuildMessageQuery()
        {
            return _context.Messages
                .Include(message => message.Sender)
                .Include(message => message.Receiver);
        }

        private async Task ValidateMessage(MessageCreateUpdateDto dto)
        {
            if (dto.SenderId.HasValue &&
                !await _context.Users.AnyAsync(user => user.Id == dto.SenderId.Value))
            {
                ModelState.AddModelError(nameof(dto.SenderId), "Odabrani posiljatelj ne postoji.");
            }

            if (dto.ReceiverId.HasValue &&
                !await _context.Users.AnyAsync(user => user.Id == dto.ReceiverId.Value))
            {
                ModelState.AddModelError(nameof(dto.ReceiverId), "Odabrani primatelj ne postoji.");
            }

            if (dto.SenderId.HasValue &&
                dto.ReceiverId.HasValue &&
                dto.SenderId.Value == dto.ReceiverId.Value)
            {
                ModelState.AddModelError(nameof(dto.ReceiverId), "Primatelj mora biti razlicit od posiljatelja.");
            }
        }

        private static void MapToEntity(MessageCreateUpdateDto dto, Message message)
        {
            message.Content = dto.Content.Trim();
            message.SentAt = dto.SentAt ?? DateTime.Now;
            message.IsRead = dto.IsRead;
            message.SenderId = dto.SenderId!.Value;
            message.ReceiverId = dto.ReceiverId!.Value;
        }
    }
}
