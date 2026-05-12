using EchoHub.Data;
using EchoHub.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace EchoHub.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public UserController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet]
        [Route("User/Dashboard")]
        public IActionResult Dashboard()
        {
           
            int userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));//CONVERS THE STRING USERID TO INT

            var role = HttpContext.Session.GetString("Role");
            ViewBag.Role = role;
            var user = _context.Users.FirstOrDefault(x => x.Id == userId);
            ViewBag.User = user;

            var submissions = _context.EwasteItems
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.DateSubmitted)
                .ToList();

            ViewData["TotalAmountPaid"] = _context.EwasteItems
            .Where(e => e.UserId == userId)
            .Sum(e => (decimal?)e.AmountPaid) ?? 0;

            ViewBag.TotalSubmission = submissions.Count();//STORE THE TOTAL NUMBER OF SUBMISSIONS IN THE VIEWBAG
            ViewBag.Recycled = submissions.Count(e => e.Status == "Recycled");//STORE THE NUMBER OF RECYCLED ITEMS IN THE VIEWBAG
            ViewBag.Pending = submissions.Count(e => e.Status == "Pending");//STORE THE NUMBER OF PENDING ITEMS IN THE VIEWBAG
            ViewBag.TotalItems = submissions.Count();//STORE THE TOTAL NUMBER OF ITEMS IN THE VIEWBAG

            return View(submissions);
        }

        [HttpGet]
        [Route("User/Submit")]
        public IActionResult Submit()//THIS IS THE GET METHOD FOR THE SUBMIT VIEW IT SHOWS THE FORM FOR THE USER TO SUBMIT THEIR E-WASTE ITEM
        {
            var sessionUserId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(sessionUserId))
                return RedirectToAction("Login", "Account");

            int userId = int.Parse(sessionUserId);

            var user = _context.Users.FirstOrDefault(x => x.Id == userId);
            ViewBag.User = user;
            ViewData["TotalAmountPaid"] = _context.EwasteItems
           .Where(e => e.UserId == userId)
           .Sum(e => (decimal?)e.AmountPaid) ?? 0;
            ViewBag.Categories = _context.Categories.ToList();
            return View();
        }

        [HttpPost]
        [Route("User/Submit")]
        [ValidateAntiForgeryToken]
        public IActionResult Submit(EwasteItem item, IFormFile file)//THIS IS THE POST METHOD FOR THE SUBMIT VIEW IT PROCESSES THE FORM DATA AND SAVES THE ITEM TO THE DATABASE
        {
            var sessionUserId = HttpContext.Session.GetString("UserId");//GET THE USERID FROM THE SESSION

            if (string.IsNullOrEmpty(sessionUserId))//IF THE USERID IN THE SESSION IS NULL OR EMPTY THEN REDIRECT TO THE LOGIN PAGE
                return RedirectToAction("Login", "Account");

            int userId = int.Parse(sessionUserId);//CONVERT THE STRING USERID TO INT

            item.UserId = userId;//SET THE USERID OF THE ITEM TO THE USERID FROM THE SESSION
            item.Status = "Pending";//SET THE STATUS OF THE ITEM TO PENDING
            item.DateSubmitted = DateTime.Now;//SET THE DATE SUBMITTED OF THE ITEM TO THE CURRENT DATE AND TIME

            if (file != null && file.Length > 0)//IF THE FILE IS NOT NULL AND THE FILE LENGTH IS GREATER THAN 0 THEN PROCESS THE FILE UPLOAD
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);//GENERATE A UNIQUE FILE NAME USING A GUID AND THE ORIGINAL FILE EXTENSION
                string folder = Path.Combine(_env.WebRootPath, "uploads");//COMBINE THE WEB ROOT PATH WITH THE UPLOADS FOLDER TO GET THE FULL PATH TO THE UPLOADS FOLDER

                if (!Directory.Exists(folder))//IF THE UPLOADS FOLDER DOES NOT EXIST THEN CREATE THE UPLOADS FOLDER
                    Directory.CreateDirectory(folder);//IF THE UPLOADS FOLDER DOES NOT EXIST THEN CREATE THE UPLOADS FOLDER

                string path = Path.Combine(folder, fileName);//COMBINE THE UPLOADS FOLDER PATH WITH THE UNIQUE FILE NAME TO GET THE FULL PATH TO THE FILE

                using (var stream = new FileStream(path, FileMode.Create))//CREATE A NEW FILE STREAM TO THE FULL PATH OF THE FILE IN CREATE MODE
                {
                    file.CopyTo(stream);//COPY THE FILE TO THE FILE STREAM
                }

                item.Image = fileName;//SET THE IMAGE PROPERTY OF THE ITEM TO THE UNIQUE FILE NAME
            }
            _context.EwasteItems.Add(item);//ADD THE ITEM TO THE DATABASE CONTEXT
            _context.SaveChanges();

            return RedirectToAction("Dashboard");
        }

        [HttpGet]
        [Route("User/MySubmission")]
        public IActionResult MySubmission(string search)//THIS IS THE MY SUBMISSION VIEW FOR THE USER TO SEE ALL THEIR SUBMISSIONS AND IT ALSO HAS A SEARCH FUNCTIONALITY TO SEARCH FOR THE SUBMISSIONS BY ITEM NAME, CATEGORY, OR STATUS
        {
            var sessionUserId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(sessionUserId))
                return RedirectToAction("Login", "Account");

            int userId = Convert.ToInt32(sessionUserId);

            var val = _context.EwasteItems
                .Where(e => e.UserId == userId);

         
            if (!string.IsNullOrWhiteSpace(search))//SEARCH FUNCTIONALITY TO SEARCH FOR THE SUBMISSIONS BY ITEM NAME, CATEGORY, OR STATUS IF THE SEARCH STRING IS NOT NULL OR EMPTY OR WHITESPACE THEN FILTER THE SUBMISSIONS BY THE SEARCH STRING
            {
                val = val.Where(e =>
                    e.Item_Name.Contains(search) ||
                    e.Category.Contains(search) ||
                    e.Status.Contains(search));
            }
            var user = _context.Users.FirstOrDefault(x => x.Id == userId);
            ViewBag.User = user;
            ViewData["TotalAmountPaid"] = _context.EwasteItems
           .Where(e => e.UserId == userId)
           .Sum(e => (decimal?)e.AmountPaid) ?? 0;
            var items = val
            .OrderByDescending(e => e.DateSubmitted)
            .ToList();

            ViewBag.Search = search;

            return View(items);
        }
        [Route("User/Profile")]
        public IActionResult Profile()
        {
            int userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            var lastEwaste = _context.EwasteItems
           .Where(e => e.UserId == userId)
           .OrderByDescending(e => e.DateSubmitted)
           .FirstOrDefault();

                ViewBag.Address = lastEwaste?.Address;

            return View(user);
        }

        [HttpPost]
        public IActionResult Update(User model, IFormFile file)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == model.Id);

            if (user == null)
                return NotFound();

            user.Name = model.Name;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;

            // PROFILE IMAGE
            if (file != null && file.Length > 0)
            {
                string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                string path = Path.Combine(_env.WebRootPath, "uploads", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                user.ProfilePicture = fileName;
            }

            _context.SaveChanges();

            return user.Role switch
            {
                "Admin" => RedirectToAction("Dashboard", "Admin"),
                "Staff" => RedirectToAction("Dashboard", "Staff"),
                _ => RedirectToAction("Dashboard", "User")
            };
        }
        public IActionResult RemovePicture(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);

            if (user != null)
            {
                user.ProfilePicture = null;
                _context.SaveChanges();
            }

            return RedirectToAction("Profile");
        }
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ChangePassword(string currentPassword,
                                            string newPassword,
                                            string confirmPassword)
        {
            int userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null)
                return RedirectToAction("Login", "Account");

            // CHECK CURRENT PASSWORD
            if (user.Password != currentPassword)
            {
                ViewBag.Error = "Current password is incorrect.";
                return View();
            }

            // CHECK MATCH
            if (newPassword != confirmPassword)
            {
                ViewBag.Error = "New password and confirm password do not match.";
                return View();
            }
            
            var reg = new Regex(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$");

            if (!reg.IsMatch(newPassword))
            {
                ModelState.AddModelError("",
                    "Password must contain uppercase, lowercase, number, special character, and be at least 8 characters.");

                return View();
            }

            // OPTIONAL VALIDATION
            if (string.IsNullOrWhiteSpace(newPassword))
            {
                ViewBag.Error = "New password cannot be empty.";
                return View();
            }

            // UPDATE PASSWORD
            user.Password = newPassword;

            _context.SaveChanges();

            ViewBag.Success = "Password changed successfully.";

            return View();
        }
    }
}