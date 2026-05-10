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

        [HttpGet]
        [Route("Staff/Dashboard")]
        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("Role") != "Staff")
            {
                return RedirectToAction("Login", "Account");
            }
            // ALL ITEMS
            var items = _context.EwasteItems
                .OrderByDescending(e => e.DateSubmitted)
                .ToList();

            // TOTAL COLLECTED
            ViewBag.TotalCollected = _context.EwasteItems
                .Count(e => e.Status == "Collected");

            // READY FOR RECYCLING
            ViewBag.ReadyForRecycling = _context.EwasteItems
                .Count(e => e.Status == "Collected");

            // DISPOSED ITEMS
            ViewBag.DisposedItems = _context.EwasteItems
                .Count(e => e.Status == "Disposed");

            // RECYCLED ITEMS
            ViewBag.RecycledItems = _context.EwasteItems
                .Count(e => e.Status == "Recycled");

            return View(items);
        }

        public IActionResult Manage()
        {
            var items = _context.EwasteItems.ToList();
            return View(items);
        }

        [HttpGet]
        [Route("Staff/ViewItem/{id}")]
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
