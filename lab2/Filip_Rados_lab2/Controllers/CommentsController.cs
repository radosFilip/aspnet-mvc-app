using Filip_Rados_lab2.Models;
using Microsoft.AspNetCore.Mvc;

namespace Filip_Rados_lab2.Controllers
{
    public class CommentsController : Controller
    {
        public IActionResult Index()
        {
            var comments = MockRepository.GetAllComments();
            return View(comments);
        }

        public IActionResult Details(int id)
        {
            var comment = MockRepository.GetCommentById(id);
            if (comment == null) return NotFound();
            return View(comment);
        }
    }
}