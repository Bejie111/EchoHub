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

            // TOTAL ITEMS
            ViewBag.TotalItems = _context.EwasteItems.Count();

            // MOST POPULAR CATEGORY
            var mostPopular = _context.EwasteItems
                .GroupBy(e => e.Category)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault();

            ViewBag.MostPopular = mostPopular ?? "N/A";

            return View(categories);
        }

        public IActionResult Create() 
        { 
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                // CHECK DUPLICATE
                bool exists = _context.Categories
                    .Any(c => c.Name.ToLower() == category.Name.ToLower());

                if (exists)
                {
                    ModelState.AddModelError("Name", "Category already exists.");
                    return View(category);
                }

                _context.Categories.Add(category);
                _context.SaveChanges();

                TempData["Success"] = "Category created successfully!";
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
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                var existing = _context.Categories.Find(category.CategoryId);

                if (existing == null)
                {
                    return NotFound();
                }

                // CHECK DUPLICATE
                bool duplicate = _context.Categories.Any(c =>
                    c.Name.ToLower() == category.Name.ToLower() &&
                    c.CategoryId != category.CategoryId);

                if (duplicate)
                {
                    ModelState.AddModelError("Name", "Category already exists.");
                    return View(category);
                }

                existing.Name = category.Name;

                _context.SaveChanges();

                TempData["Success"] = "Category updated successfully!";
                return RedirectToAction("Index");
            }

            return View(category);
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
