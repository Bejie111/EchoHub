using EchoHub.Data;
using EchoHub.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EchoHub.Controllers
{
    public class CollectionController : Controller
    {
        private readonly AppDbContext _context;

        public CollectionController(AppDbContext context)
        {
            _context = context;
        }

        // DASHBOARD
        public IActionResult Collect()
        {
            // Pending E-waste items not yet assigned
            ViewBag.PendingItems = _context.EwasteItems
                .Include(e => e.User)
                .Where(e => !_context.Collections.Any(c => c.EwasteId == e.EwasteId))
                .ToList();

            // Scheduled Collections
            var scheduled = _context.Collections
                .Include(c => c.EwasteItem)
                    .ThenInclude(e => e.User)
                .Include(c => c.Staff)
                .Where(c => c.Status == "Scheduled")
                .ToList();

            return View(scheduled);
        }

        // GET: Assign Collection
        public IActionResult Assign(int id)
        {
            var item = _context.EwasteItems.Find(id);

            if (item == null)
                return NotFound();

            ViewBag.EwasteId = id;
            ViewBag.ItemName = item.Item_Name;

            ViewBag.StaffList = new SelectList(
                _context.Users.Where(u => u.Role == "Staff"),
                "UserId",
                "Name"
            );

            return View();
        }

        // POST: Assign Collection
        [HttpPost]
        public IActionResult Assign(Collection collection)
        {
            if (ModelState.IsValid)
            {
                collection.Status = "Scheduled";

                _context.Collections.Add(collection);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.StaffList = new SelectList(
                _context.Users.Where(u => u.Role == "Staff"),
                "UserId",
                "Name"
            );

            return View(collection);
        }

        // Mark as Collected
        public IActionResult MarkCollected(int id)
        {
            var collect = _context.Collections.Find(id);

            if (collect != null)
            {
                collect.Status = "Collected";
                collect.CollectionDate = DateTime.Now;

                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}