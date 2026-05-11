using Microsoft.AspNetCore.Mvc;
using EchoHub.Models;
using EchoHub.Data;

namespace EchoHub.Controllers
{
    public class CategoryController : Controller
    {
       private readonly AppDbContext _context;
       
        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("Category/Index")]
        public IActionResult Index()
        {
            //AUTH CHECK - ONLY ADMIN CAN ACCESS, IT CHECKS THE SESSION FOR THE ROLE, IF NOT ADMIN, REDIRECT TO LOGIN
            if (HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            var categories = _context.Categories.ToList();

            // TOTAL ITEMS - COUNT ALL ITEMS IN THE DATABASE
            ViewBag.TotalItems = _context.EwasteItems.Count();

            // MOST POPULAR CATEGORY - GROUP BY CATEGORY, COUNT ITEMS IN EACH CATEGORY, ORDER BY COUNT DESCENDING, TAKE FIRST
            var mostPopular = _context.EwasteItems
                .GroupBy(e => e.Category)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault();

            ViewBag.MostPopular = mostPopular ?? "N/A";

            return View(categories);
        }

        [HttpGet]
        [Route("Category/Create")]
        public IActionResult Create() // GET method to show the create form
        { 
            return View();
        }

        [HttpPost]
        [Route("Category/Create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category) // POST method to handle form submission
        {
            if (ModelState.IsValid)
            {
                // CHECK DUPLICATE - CHECK IF A CATEGORY WITH THE SAME NAME ALREADY EXISTS IN THE DATABASE, IGNORE CASE
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

        // EDIT - // GET METHOD TO SHOW THE EDIT FORM, POST METHOD TO HANDLE FORM SUBMISSION
        [HttpGet]
        [Route("Category/Edit/{CategoryId}")]
        public IActionResult Edit(int CategoryId)
        {
            var category = _context.Categories.Find(CategoryId);
            return View(category);
        }

        [HttpPost]
        [Route("Category/Edit/{CategoryId}")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category) //POST METHOD TO HANDLE FORM SUBMISSION
        {
            // CHECK IF MODEL STATE IS VALID - THIS CHECKS IF THE DATA SUBMITTED IN THE FORM MEETS THE VALIDATION RULES DEFINED IN THE MODEL (E.G., REQUIRED FIELDS, STRING LENGTH, ETC.)
            if (ModelState.IsValid)
            {
                var existing = _context.Categories.Find(category.CategoryId);

                if (existing == null)
                {
                    return NotFound();
                }

                // CHECK DUPLICATE - CHECK IF A CATEGORY WITH THE SAME NAME ALREADY EXISTS IN THE DATABASE, IGNORE CASE, EXCLUDE THE CURRENT CATEGORY BEING EDITED
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
        [HttpGet]
        [Route("Category/Delete/{CategoryId}")]
        public IActionResult Delete(int CategoryId) //DELETE METHOD TO HANDLE DELETION
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
