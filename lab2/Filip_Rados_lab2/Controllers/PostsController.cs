using Filip_Rados_lab2.Models;
using Microsoft.AspNetCore.Mvc;

namespace Filip_Rados_lab2.Controllers
{
    public class PostsController : Controller
    {
        public IActionResult Index(string? category)
        {
            var posts = MockRepository.GetAllPosts();

            if (!string.IsNullOrEmpty(category) &&
                Enum.TryParse<PostCategory>(category, out var cat))
            {
                posts = MockRepository.GetPostsByCategory(cat);
                ViewBag.ActiveCategory = category;
            }

            return View(posts);
        }

        public IActionResult Details(int id)
        {
            var post = MockRepository.GetPostById(id);
            if (post == null) return NotFound();

            ViewBag.LikeCount = MockRepository.GetLikeCountForPost(id);
            return View(post);
        }
    }
}