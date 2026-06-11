using Filip_Rados_lab5.Data;
using Filip_Rados_lab5.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace Filip_Rados_lab5.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.RecentPosts = await _context.Posts
                .Include(post => post.Author)
                    .ThenInclude(author => author.ProfileImages)
                .Include(post => post.Comments)
                .Include(post => post.Tags)
                .Include(post => post.Likes)
                .OrderByDescending(post => post.CreatedAt)
                .Take(10)
                .ToListAsync();

            ViewBag.TotalPosts = await _context.Posts.CountAsync();
            ViewBag.TotalUsers = await _context.Users.CountAsync();
            ViewBag.TotalComments = await _context.Comments.CountAsync();
            ViewBag.TotalLikes = await _context.Likes.CountAsync();
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewBag.CurrentUser = int.TryParse(currentUserId, out var userId)
                ? await _context.Users
                    .Include(user => user.ProfileImages)
                    .FirstOrDefaultAsync(user => user.Id == userId)
                : null;
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
