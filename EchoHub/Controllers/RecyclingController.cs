using EchoHub.Data;
using EchoHub.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EchoHub.Controllers
{
    public class RecyclingController : Controller
    {
        private readonly AppDbContext _context;

        public RecyclingController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Dashboard()
        {
            var readyItems = _context.EwasteItems
                .Where(e => e.Status == "Collected")
                .ToList();

            var recycledItems = _context.EwasteItems
                .Where(e => e.Status == "Recycled")
                .ToList();

            var disposedItems = _context.EwasteItems
                .Where(e => e.Status == "Disposed")
                .ToList();

            ViewBag.ReadyCount = readyItems.Count;
            ViewBag.RecycledCount = recycledItems.Count;
            ViewBag.DisposedCount = disposedItems.Count;

            ViewBag.ReadyItems = readyItems;

            var records = _context.EwasteItems
                .Where(e => e.Status == "Recycled" || e.Status == "Disposed")
                .OrderByDescending(e => e.DateSubmitted)
                .ToList();

            return View(records);
        }

        [HttpGet]
        public IActionResult Record()
        {
            var item = _context.EwasteItems
                .Where(e => e.Status == "Collected")
                .ToList();
            if (item == null) return NotFound();

            ViewBag.ItemList = new SelectList(item, "EwasteId", "Item_Name");
            return View();
        }

        [HttpPost]
        public IActionResult Record(int EwasteId, string Status)
        {
            var item = _context.EwasteItems.Find(EwasteId);

           if(item == null) return NotFound();

            if (Status != "Recycled" && Status != "Disposed") 
            {
                ModelState.AddModelError("Status", "Invalid status. Please select either 'Recycled' or 'Disposed'.");
            }

            if (ModelState.IsValid)
            { 
                item.Status = Status;
                _context.SaveChanges();
                return RedirectToAction("Dashboard");

            }

            var readyItems = _context.EwasteItems
                .Where(e => e.Status == "Collected")
                .ToList();

            ViewBag.ItemList = new SelectList(readyItems, "EwasteId", "Item_Name");

            return View();
        }

    }
}
