using EchoHub.Data;
using EchoHub.Models;
using Microsoft.AspNetCore.Mvc;

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

        public IActionResult View(int Ewasiteid)
        {
            var item = _context.EwasteItems.FirstOrDefault(x => x.EwasteId == Ewasiteid);
            return View(item);
        }

        public IActionResult Update(int EwasteId)
        {
            var item = _context.EwasteItems.Find(EwasteId);
            return View(item);
        }

        [HttpPost]
        public IActionResult Update(EwasteItem updated)
        {
            var item = _context.EwasteItems.Find(updated.EwasteId);

            if (item != null)
            {
                item.Item_Name = updated.Item_Name;
                item.Category = updated.Category;
                item.Description = updated.Description;
                item.Status = updated.Status;
            }

            _context.SaveChanges();

            return RedirectToAction("Manage");
        }


    }
}
