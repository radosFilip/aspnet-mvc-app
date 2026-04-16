using Filip_Rados_lab2.Models;
using Microsoft.AspNetCore.Mvc;

namespace Filip_Rados_lab2.Controllers
{
    public class UsersController : Controller
    {
        public IActionResult Index()
        {
            var users = MockRepository.GetAllUsers();
            return View(users);
        }

        public IActionResult Details(int id)
        {
            var user = MockRepository.GetUserById(id);
            if (user == null) return NotFound();

            ViewBag.Posts = MockRepository.GetPostsByUser(id);
            ViewBag.Comments = MockRepository.GetCommentsByUser(id);
            ViewBag.Following = MockRepository.GetFollowing(id);
            ViewBag.Followers = MockRepository.GetFollowers(id);
            return View(user);
        }
    }
}