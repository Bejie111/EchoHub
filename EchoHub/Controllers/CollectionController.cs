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

        // DASHBOARD - VIEW ALL THE PENDING COLLECTIONS
        public IActionResult Collect()
        {
            //PENDING ITEMS - NOT ASSIGNED TO ANY COLLECTION
            ViewBag.PendingItems = _context.EwasteItems
                .Include(e => e.User)
                .Where(e => e.Status == "Pending" && !_context.Collections.Any(c => c.EwasteId == e.EwasteId)) 
                .ToList();

            //ALL COLLECTIONS - INCLUDING ASSIGNED AND PENDING
            var collections = _context.Collections
                .Include(c => c.EwasteItem)
                    .ThenInclude(e => e.User)
                .OrderByDescending(c => c.ScheduleDate)
                .ToList();
            return View(collections);
        }

        // GET: ASSIGN COLLECTION - SHOW THE FORM TO ASSIGN A COLLECTION TO AN E-WASTE ITEM
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

        // POST: ASSIGN COLLECTION - HANDLE THE FORM SUBMISSION TO ASSIGN A COLLECTION TO AN E-WASTE ITEM
        [HttpPost]
        public IActionResult Assign(Collection collection)
        {
            if (ModelState.IsValid) //VALIDATE THE MODEL AND IF VALID, CREATE A NEW COLLECTION RECORD WITH STATUS "Scheduled" AND NULL COLLECTION DATE
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

        // MARK AS COLLECTED - MARK A COLLECTION AS COLLECTED AND UPDATE THE STATUS OF THE E-WASTE ITEM
        public IActionResult MarkCollected(int id)
        {
            var collect = _context.Collections
                .Include(c => c.EwasteItem)
                .FirstOrDefault(c => c.CollectionId == id);

            if (collect != null) //IF THE COLLECTION RECORD EXISTS, UPDATE ITS STATUS TO "Collected", SET THE COLLECTION DATE TO THE CURRENT DATE, AND UPDATE THE STATUS OF THE ASSOCIATED E-WASTE ITEM TO "Collected"
            {
                collect.Status = "Collected";
                collect.CollectionDate = DateTime.Now;

                if(collect.EwasteItem != null)//IF THE ASSOCIATED E-WASTE ITEM EXISTS, UPDATE ITS STATUS TO "Collected" AS WELL
                {
                    collect.EwasteItem.Status = "Collected";
                }

                _context.SaveChanges();
            }

            return RedirectToAction("Collect");
        }
    }
}