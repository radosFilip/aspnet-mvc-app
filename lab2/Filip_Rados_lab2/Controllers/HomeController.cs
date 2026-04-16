using Filip_Rados_lab2.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Filip_Rados_lab2.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.RecentPosts = DataSeeder.Posts
                .OrderByDescending(p => p.CreatedAt)
                .Take(10)
                .ToList();
            ViewBag.TotalPosts    = DataSeeder.Posts.Count;
            ViewBag.TotalUsers    = DataSeeder.Users.Count;
            ViewBag.TotalComments = DataSeeder.Comments.Count;
            ViewBag.TotalLikes    = DataSeeder.Likes.Count;
            ViewBag.Likes         = DataSeeder.Likes;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
