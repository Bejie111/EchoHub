using Microsoft.AspNetCore.Mvc;
using EchoHub.Models;
using EchoHub.Data;
using System.Linq;

namespace EchoHub.Controllers
{
    public class CategoryController : Controller
    {
       private readonly AppDbContext _context;
       
        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }
            var categories = _context.Categories.ToList();
            return View(categories);
        }

        public IActionResult Create() 
        { 
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Add(category);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        // EDIT
        public IActionResult Edit(int CategoryId)
        {
            var category = _context.Categories.Find(CategoryId);
            return View(category);
        }

        [HttpPost]
        public IActionResult Edit(Category category)
        {
            var existing = _context.Categories.Find(category.CategoryId);

            if (existing != null)
            {
                existing.Name = category.Name;
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        // DELETE
        public IActionResult Delete(int CategoryId)
        {
            var category = _context.Categories.Find(CategoryId);

            if (category != null)
            {
                _context.Categories.Remove(category);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

    }
}
