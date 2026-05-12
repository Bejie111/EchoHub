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
            var role = HttpContext.Session.GetString("Role");
            ViewBag.Role = role;
            var userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            var user = _context.Users.FirstOrDefault(x => x.Id == userId);
            ViewBag.User = user;
            // ALL ITEMS ORDERED BY MOST RECENT
            var items = _context.EwasteItems
                .OrderByDescending(e => e.DateSubmitted)
                .ToList();

            // TOTAL COLLECTED ITEMS
            ViewBag.TotalCollected = _context.EwasteItems
                .Count(e => e.Status == "Collected");

            // READY FOR RECYCLING ITEMS
            ViewBag.ReadyForRecycling = _context.EwasteItems
                .Count(e => e.Status == "Collected");

            // TOTAL DISPOSED ITEMS 
            ViewBag.DisposedItems = _context.EwasteItems
                .Count(e => e.Status == "Disposed");

            // TOTAL RECYCLED ITEMS
            ViewBag.RecycledItems = _context.EwasteItems
                .Count(e => e.Status == "Recycled");

            return View(items);
        }

        public IActionResult Manage()// THIS IS THE MANAGE VIEW FOR STAFF TO SEE ALL THE ITEMS IN THE SYSTEM
        {
            var userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            var user = _context.Users.FirstOrDefault(x => x.Id == userId);
            ViewBag.User = user;
            var items = _context.EwasteItems.ToList();
            return View(items);
        }

        [HttpGet]
        [Route("Staff/ViewItem/{id}")]
        public IActionResult ViewItem(int id) //THIS IS THE VIEW ITEM FOR STAFF TO SEE THE DETAILS OF THE ITEM
        {
            var userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            var user = _context.Users.FirstOrDefault(x => x.Id == userId);
            ViewBag.User = user;
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
