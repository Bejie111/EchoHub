using EchoHub.Data;
using Microsoft.AspNetCore.Mvc;
using EchoHub.Models;
using System.Linq;

namespace EchoHub.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Users(string search)
        {
            var users = _context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                users = users.Where(u =>
                    u.Name.Contains(search) ||
                    u.Email.Contains(search));
            }

            return View(users.ToList());
        }
        public IActionResult EditUser(int id)
        {
            var user = _context.Users.Find(id);
            return View(user);
        }


        [HttpPost]
        public IActionResult EditUser(User updatedUser)
        {
            var user = _context.Users.Find(updatedUser.Id);

            if (user != null)
            {
                user.Name = updatedUser.Name;
                user.Email = updatedUser.Email;
                user.Role = updatedUser.Role;

                _context.SaveChanges();
            }

            return RedirectToAction("Users");
        }

        public IActionResult DeleteUser(int id)
        {
            var user = _context.Users.Find(id);

            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }

            return RedirectToAction("Users");
        }

        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }



    }
}
