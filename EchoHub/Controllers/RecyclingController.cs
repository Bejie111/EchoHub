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
        public IActionResult Dashboard() //VIEW ALL THE ITEMS THAT ARE READY FOR RECYCLING OR DISPOSAL, AND ALSO SHOW THE COUNT OF ITEMS IN EACH STATUS
        {
            var readyItems = _context.EwasteItems
                .Where(e => e.Status == "Collected") //ONLY SHOW THE ITEMS THAT ARE READY FOR RECYCLING OR DISPOSAL (STATUS "Collected")
                .ToList();

            var recycledItems = _context.EwasteItems
                .Where(e => e.Status == "Recycled") //ONLY SHOW THE ITEMS THAT HAVE BEEN RECYCLED (STATUS "Recycled")
                .ToList();

            var disposedItems = _context.EwasteItems
                .Where(e => e.Status == "Disposed")//ONLY SHOW THE ITEMS THAT HAVE BEEN DISPOSED (STATUS "Disposed")
                .ToList();

            ViewBag.ReadyCount = readyItems.Count;//STORE THE COUNT OF READY ITEMS IN THE VIEWBAG TO DISPLAY ON THE DASHBOARD
            ViewBag.RecycledCount = recycledItems.Count;//STORE THE COUNT OF RECYCLED ITEMS IN THE VIEWBAG TO DISPLAY ON THE DASHBOARD
            ViewBag.DisposedCount = disposedItems.Count;//STORE THE COUNT OF DISPOSED ITEMS IN THE VIEWBAG TO DISPLAY ON THE DASHBOARD

            ViewBag.ReadyItems = readyItems;//STORE THE LIST OF READY ITEMS IN THE VIEWBAG TO DISPLAY ON THE DASHBOARD

            var records = _context.EwasteItems
                .Where(e => e.Status == "Recycled" || e.Status == "Disposed")//ONLY SHOW THE ITEMS THAT HAVE BEEN RECYCLED OR DISPOSED (STATUS "Recycled" OR "Disposed")
                .OrderByDescending(e => e.DateSubmitted)//OREDER THE ITEMS BY THE DATE THEY WERE SUBMITTED, WITH THE MOST RECENT ITEMS FIRST
                .ToList();

            return View(records);
        }

        [HttpGet]
        public IActionResult Record()//SHOW THE FORM TO RECORD THE RECYCLING OR DISPOSAL OF AN ITEM
        {
            var item = _context.EwasteItems
                .Where(e => e.Status == "Collected")
                .ToList();
            if (item == null) return NotFound();

            ViewBag.ItemList = new SelectList(item, "EwasteId", "Item_Name");//RELOAD THE FORM WITH THE LIST OF READY ITEMS TO SELECT FROM
            return View();
        }

        [HttpPost]
        public IActionResult Record(int EwasteId, string Status)//HANDLE THE FORM SUBMISSION TO RECORD THE RECYCLING OR DISPOSAL OF AN ITEM
        {
            var item = _context.EwasteItems.Find(EwasteId);

           if(item == null) return NotFound();//CHECK IF THE ITEM EXISTS IN THE DATABASE, IF NOT RETURN A 404 NOT FOUND RESPONSE

            if (Status != "Recycled" && Status != "Disposed") //CHECH IF THE SELECTED STATUS IS VALID, IF NOT ADD A MODEL ERROR TO THE MODELSTATE TO DISPLAY AN ERROR MESSAGE ON THE FORM
            {
                ModelState.AddModelError("Status", "Invalid status. Please select either 'Recycled' or 'Disposed'.");
            }

            if (ModelState.IsValid)//VALIDATE THE MODEL AND IF VALID, UPDATE THE STATUS OF THE E-WASTE ITEM TO "Recycled" OR "Disposed" BASED ON THE SELECTED STATUS AND SAVE THE CHANGES TO THE DATABASE
            { 
                item.Status = Status;
                _context.SaveChanges();
                return RedirectToAction("Dashboard");

            }

            var readyItems = _context.EwasteItems
                .Where(e => e.Status == "Collected")
                .ToList();

            ViewBag.ItemList = new SelectList(readyItems, "EwasteId", "Item_Name");//RELOAD THE FORM WITH THE LIST OF READY ITEMS TO SELECT FROM 

            return View();
        }

    }
}
