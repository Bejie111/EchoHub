using EchoHub.Data;
using EchoHub.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EchoHub.Controllers
{
    public class StaffController : Controller
    {
        private readonly AppDbContext _context;

        public StaffController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("Role") != "Staff")
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        public IActionResult Manage()
        {
            var items = _context.EwasteItems.ToList();
            return View(items);
        }

        public IActionResult ViewItem(int id)
        {
            var item = _context.EwasteItems
                .Include(e => e.User)
                .FirstOrDefault(e => e.EwasteId == id);

            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }
    }
}
