using EchoHub.Data;
using Microsoft.AspNetCore.Mvc;
using EchoHub.Models;

namespace EchoHub.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AdminController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }


        //USERS MANAGEMENT
        [HttpGet]
        [Route("Admin/Users")]
        public IActionResult Users(string search)
        {
            var userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            var user = _context.Users.FirstOrDefault(x => x.Id == userId);

            ViewBag.User = user;
            var users = _context.Users.AsQueryable();
            //SUBMISSION - Counts how many e-waste items each user has submitted and passes this data to the view using ViewBag.
            ViewBag.SubmissionCounts = _context.EwasteItems
                .GroupBy(e => e.UserId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Count()
                );

            return View(users.ToList());
        }

        //USER EDIT - Displays a form pre-populated with the user's current details, allowing the admin to make changes.
        [HttpGet]
        [Route("Admin/EditUser/{id}")]
        public IActionResult EditUser(int id)
        {
            var userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            var user = _context.Users.FirstOrDefault(x => x.Id == userId);

            ViewBag.User = user;
            var users = _context.Users.Find(id);
            return View(users);
        }

        //USER UPDATE - Handles the form submission from the EditUser view, updating the user's details in the database.
        [HttpPost]
        [Route("Admin/EditUser/{id}")]
        public IActionResult EditUser(User updatedUser)
        {
            var user = _context.Users.Find(updatedUser.Id);

            if (user != null)
            {
                user.Name = updatedUser.Name;
                user.Email = updatedUser.Email;
                user.Role = updatedUser.Role;

                _context.SaveChanges();
            }

            return RedirectToAction("Users");
        }
        //USER DELETE - Handles the deletion of a user from the database.
        [HttpGet]
        [Route("Admin/DeleteUser/{id}")]
        public IActionResult DeleteUser(int id)
        {
            var user = _context.Users.Find(id);

            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }

            return RedirectToAction("Users");
        }

        //DASHBOARD - Displays key statistics and visualizations about e-waste submissions, recycling rates, and user activity.
        [HttpGet]
        [Route("Admin/Dashboard")]
        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("Login", "Account");
            var role = HttpContext.Session.GetString("Role");
            ViewBag.Role = role;
            ViewBag.TotalAmountPaid = null;

            var userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            var user = _context.Users.FirstOrDefault(x => x.Id == userId);
            ViewBag.User = user;
            // Stats
            ViewBag.TotalUsers = _context.Users.Count();
            ViewBag.TotalItems = _context.EwasteItems.Count();
            ViewBag.TotalRecycled = _context.EwasteItems.Count(e => e.Status == "Recycled");
            ViewBag.TotalDisposed = _context.EwasteItems.Count(e => e.Status == "Disposed");

            // Recent Users (latest 5)
            ViewBag.RecentUsers = _context.Users
                                    .OrderByDescending(u => u.Id)
                                    .Take(5)
                                    .ToList();

            // Monthly data for charts (last 6 months)
            var months = Enumerable.Range(0, 6)
                .Select(i => DateTime.Now.AddMonths(-5 + i))
                .ToList();

            var monthlySubmissions = months
                .Select(m => _context.EwasteItems
                    .Count(e => e.DateSubmitted.Month == m.Month
                             && e.DateSubmitted.Year == m.Year))
                .ToList();

            var monthlyRecycled = months
                .Select(m => _context.EwasteItems
                    .Count(e => e.Status == "Recycled"
                             && e.DateSubmitted.Month == m.Month
                             && e.DateSubmitted.Year == m.Year))
                .ToList();

            var monthlyDisposed = months
                .Select(m => _context.EwasteItems
                    .Count(e => e.Status == "Disposed"
                             && e.DateSubmitted.Month == m.Month
                             && e.DateSubmitted.Year == m.Year))
                .ToList();

            ViewBag.MonthlySubmissions = System.Text.Json.JsonSerializer.Serialize(monthlySubmissions);
            ViewBag.MonthlyRecycled = System.Text.Json.JsonSerializer.Serialize(monthlyRecycled);
            ViewBag.MonthlyDisposed = System.Text.Json.JsonSerializer.Serialize(monthlyDisposed);

            //CATEGORY DISTRIBUTION - Groups e-waste items by category and counts how many items fall into each category, passing this data to the view for visualization.
            var categoryData = _context.EwasteItems
                .GroupBy(e => e.Category)
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .ToList();

            ViewBag.CategoryLabels = System.Text.Json.JsonSerializer.Serialize(
                categoryData.Select(c => c.Category).ToList());
            ViewBag.CategoryData = System.Text.Json.JsonSerializer.Serialize(
                categoryData.Select(c => c.Count).ToList());

            return View();
        }
    }
}