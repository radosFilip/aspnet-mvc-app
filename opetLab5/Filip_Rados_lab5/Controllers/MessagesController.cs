using Filip_Rados_lab5.Data;
using Filip_Rados_lab5.Models;
using Filip_Rados_lab5.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Filip_Rados_lab5.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MessagesController : Controller
    {
        private readonly AppDbContext _context;

        public MessagesController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? q)
        {
            var messages = await BuildQuery(q).ToListAsync();
            ViewBag.Query = q;
            return View(messages);
        }

        public async Task<IActionResult> Search(string? q)
        {
            var messages = await BuildQuery(q).ToListAsync();
            return PartialView("_List", messages);
        }

        public async Task<IActionResult> Details(int id)
        {
            var message = await _context.Messages
                .Include(item => item.Sender)
                .Include(item => item.Receiver)
                .FirstOrDefaultAsync(item => item.Id == id);

            if (message == null) return NotFound();
            return View(message);
        }

        public IActionResult Create()
        {
            return View(new MessageFormModel
            {
                SentAt = DateTime.Now
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MessageFormModel model)
        {
            await ValidateMessageForm(model);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var message = new Message();
            MapToEntity(model, message);

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            TempData["Flash"] = "Poruka je uspjesno kreirana.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var message = await _context.Messages
                .Include(item => item.Sender)
                .Include(item => item.Receiver)
                .FirstOrDefaultAsync(item => item.Id == id);

            if (message == null) return NotFound();
            return View(MapToForm(message));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MessageFormModel model)
        {
            if (id != model.Id) return NotFound();

            var message = await _context.Messages.FindAsync(id);
            if (message == null) return NotFound();

            await ValidateMessageForm(model);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            MapToEntity(model, message);
            await _context.SaveChangesAsync();

            TempData["Flash"] = "Poruka je uspjesno azurirana.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var message = await _context.Messages
                .Include(item => item.Sender)
                .Include(item => item.Receiver)
                .FirstOrDefaultAsync(item => item.Id == id);

            if (message == null) return NotFound();
            return View(message);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message == null) return NotFound();

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();

            TempData["Flash"] = "Poruka je obrisana.";
            return RedirectToAction(nameof(Index));
        }

        private IQueryable<Message> BuildQuery(string? q)
        {
            q = q?.Trim();

            var query = _context.Messages
                .Include(message => message.Sender)
                .Include(message => message.Receiver)
                .AsQueryable();

            if (!string.IsNullOrEmpty(q))
            {
                query = query.Where(message =>
                    EF.Functions.Like(message.Content, $"%{q}%") ||
                    EF.Functions.Like(message.Sender.UserName, $"%{q}%") ||
                    EF.Functions.Like(message.Receiver.UserName, $"%{q}%"));
            }

            return query.OrderByDescending(message => message.SentAt);
        }

        private async Task ValidateMessageForm(MessageFormModel model)
        {
            if (model.SenderId.HasValue &&
                !await _context.Users.AnyAsync(user => user.Id == model.SenderId.Value))
            {
                ModelState.AddModelError(nameof(model.SenderId), "Odabrani posiljatelj ne postoji.");
            }

            if (model.ReceiverId.HasValue &&
                !await _context.Users.AnyAsync(user => user.Id == model.ReceiverId.Value))
            {
                ModelState.AddModelError(nameof(model.ReceiverId), "Odabrani primatelj ne postoji.");
            }

            if (model.SenderId.HasValue &&
                model.ReceiverId.HasValue &&
                model.SenderId.Value == model.ReceiverId.Value)
            {
                ModelState.AddModelError(nameof(model.ReceiverId), "Primatelj mora biti razlicit od posiljatelja.");
            }
        }

        private static MessageFormModel MapToForm(Message message)
        {
            return new MessageFormModel
            {
                Id = message.Id,
                Content = message.Content,
                SentAt = message.SentAt,
                IsRead = message.IsRead,
                SenderId = message.SenderId,
                SenderText = message.Sender == null ? null : "@" + message.Sender.UserName,
                ReceiverId = message.ReceiverId,
                ReceiverText = message.Receiver == null ? null : "@" + message.Receiver.UserName
            };
        }

        private static void MapToEntity(MessageFormModel model, Message message)
        {
            message.Content = model.Content.Trim();
            message.SentAt = model.SentAt!.Value;
            message.IsRead = model.IsRead;
            message.SenderId = model.SenderId!.Value;
            message.ReceiverId = model.ReceiverId!.Value;
        }
    }
}
