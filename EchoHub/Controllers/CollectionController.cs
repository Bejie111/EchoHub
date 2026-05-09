using EchoHub.Data;
using EchoHub.Models;
using Microsoft.AspNetCore.Mvc;
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
           //Pending 
           ViewBag.PendingItems = _context.EwasteItems
                .Include(e => e.User)
                .Where(e => e.Status == "Pending" && !_context.Collections.Any(c => c.EwasteId == e.EwasteId))
                .ToList();

            var collections = _context.Collections
                .Include(c => c.EwasteItem)
                    .ThenInclude(e => e.User)
                .OrderByDescending(c => c.ScheduleDate)
                .ToList();
            return View(collections);
        }

        // GET: Assign Collection
        public IActionResult Assign(int id)
        {
            var item = _context.EwasteItems.Find(id);

            if (item == null)
                return NotFound();

            var model = new Collection
            {
                EwasteId = id 
            };

            ViewBag.ItemName = item.Item_Name;
            return View(model);
        }

        // POST: Assign Collection
        [HttpPost]
        public IActionResult Assign(Collection collection)
        {
            if (ModelState.IsValid)
            {
                collection.Status = "Scheduled";
                collection.CollectionDate = null;

                _context.Collections.Add(collection);
                _context.SaveChanges();

                return RedirectToAction("Collect");

            }
            var item = _context.EwasteItems.Find(collection.EwasteId);
            ViewBag.ItemName = item?.Item_Name;

            return View(collection);
        }

        // Mark as Collected
        public IActionResult MarkCollected(int id)
        {
            var collect = _context.Collections
                .Include(c => c.EwasteItem)
                .FirstOrDefault(c => c.CollectionId == id);

            if (collect != null)
            {
                collect.Status = "Collected";
                collect.CollectionDate = DateTime.Now;

                if(collect.EwasteItem != null)
                {
                    collect.EwasteItem.Status = "Collected";
                }

                _context.SaveChanges();
            }

            return RedirectToAction("Collect");
        }
    }
}