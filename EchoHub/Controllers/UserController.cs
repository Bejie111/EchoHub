using EchoHub.Data;
using EchoHub.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EchoHub.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public UserController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Dashboard()
        {
           
            int userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));

            var submissions = _context.EwasteItems
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.DateSubmitted)
                .ToList();

            ViewBag.TotalSubmission = submissions.Count();
            ViewBag.Recycled = submissions.Count(e => e.Status == "Recycled");
            ViewBag.Pending = submissions.Count(e => e.Status == "Pending");
            ViewBag.TotalItems = submissions.Count();

            return View(submissions);
        }

        public IActionResult Submit()
        {
            ViewBag.Categories = _context.Categories.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Submit(EwasteItem item, IFormFile file)
        {
            var sessionUserId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(sessionUserId))
                return RedirectToAction("Login", "Account");

            int userId = int.Parse(sessionUserId);

            item.UserId = userId;
            item.Status = "Pending";
            item.DateSubmitted = DateTime.Now;

            if (file != null && file.Length > 0)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string folder = Path.Combine(_env.WebRootPath, "uploads");

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                string path = Path.Combine(folder, fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                item.Image = fileName;
            }
            _context.EwasteItems.Add(item);
            _context.SaveChanges();

            return RedirectToAction("Dashboard");
        }

        public IActionResult MySubmission(string search)
        {
            var sessionUserId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(sessionUserId))
                return RedirectToAction("Login", "Account");

            int userId = Convert.ToInt32(sessionUserId);

            var val = _context.EwasteItems
                .Where(e => e.UserId == userId);

         
            if (!string.IsNullOrWhiteSpace(search))
            {
                val = val.Where(e =>
                    e.Item_Name.Contains(search) ||
                    e.Category.Contains(search) ||
                    e.Status.Contains(search));
            }

            var items = val
                .OrderByDescending(e => e.DateSubmitted)
                .ToList();

            ViewBag.Search = search;

            return View(items);
        }
    }
}