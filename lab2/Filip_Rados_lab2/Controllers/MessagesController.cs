using Filip_Rados_lab2.Models;
using Microsoft.AspNetCore.Mvc;

namespace Filip_Rados_lab2.Controllers
{
    public class MessagesController : Controller
    {
        public IActionResult Index()
        {
            var messages = MockRepository.GetAllMessages();
            return View(messages);
        }

        public IActionResult Details(int id)
        {
            var message = MockRepository.GetMessageById(id);
            if (message == null) return NotFound();
            return View(message);
        }
    }
}