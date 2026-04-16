using Filip_Rados_lab2.Models;
using Microsoft.AspNetCore.Mvc;

namespace Filip_Rados_lab2.Controllers
{
    public class TagsController : Controller
    {
        public IActionResult Index()
        {
            var tags = MockRepository.GetAllTags();
            return View(tags);
        }

        public IActionResult Details(int id)
        {
            var tag = MockRepository.GetTagById(id);
            if (tag == null) return NotFound();

            ViewBag.Posts = MockRepository.GetPostsByTag(id);
            return View(tag);
        }
    }
}