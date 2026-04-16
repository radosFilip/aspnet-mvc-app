using Filip_Rados_lab2.Models;
using Microsoft.AspNetCore.Mvc;

namespace Filip_Rados_lab2.Controllers
{
    public class ReportsController : Controller
    {
        public IActionResult Index()
        {
            var reports = MockRepository.GetAllReports();
            return View(reports);
        }

        public IActionResult Details(int id)
        {
            var report = MockRepository.GetReportById(id);
            if (report == null) return NotFound();
            return View(report);
        }
    }
}