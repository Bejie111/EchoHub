using EchoHub.Data;
using EchoHub.Models;
using Microsoft.AspNetCore.Mvc;

namespace EchoHub.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }


        //Displays the login form for the Users.
        [HttpGet]
        [Route("Account/Login")]
        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        [Route("Account/Login")]
        public IActionResult Login(string email, string password) 
        {
            User foundUser = null;


            //Find a recode that maches both Email and Password
            foreach (var u in _context.Users)
            {
                if (u.Email == email && u.Password == password)
                { 
                    foundUser = u;
                    break;//If match found, exit the loop 
                }
            }

            //AUTHENTICATION CHECK:
            if (foundUser != null) 
            {
                //Store critical  user data  in Session so it is accessible on other pages.
                //This prevents unauthorized access to role-specific routes.
                HttpContext.Session.SetString("Role", foundUser.Role);
                HttpContext.Session.SetString("UserId", foundUser.Id.ToString());

                //Redirects the user to a specific dashboard based on their access level
                if (foundUser.Role == "Admin") return RedirectToAction("Dashboard", "Admin");
                if (foundUser.Role == "Staff") return RedirectToAction("Dashboard", "Staff");
                
                // Default redirect for regular users
                return RedirectToAction("Dashboard", "User");
            }

            //If no user was found, pass an error message to the view without redirecting
            ViewBag.Error = "Invalid login credentials";
            return View();
        }


        //Displays the registration form for new account creation.
        [HttpGet]
        [Route("Account/Register")]
        public IActionResult Register()
        {
            return View();
        }


        //Handles the creation of a new user.
        [HttpPost]
        [Route("Account/Register")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(User user)
        {
            //Checks if the data provided matches the requirements set in the User Model
            if (ModelState.IsValid)
            {
                // CHECK IF EMAIL ALREADY EXISTS
                bool emailExists = _context.Users
                    .Any(u => u.Email.ToLower() == user.Email.ToLower());

                if (emailExists)
                {
                    ModelState.AddModelError("Email",
                        "This email is already registered.");
                    return View(user);
                }

                // SAVE LOWERCASE EMAIL
                user.Email = user.Email.ToLower();

                _context.Users.Add(user);
                _context.SaveChanges();

                return RedirectToAction("Login");
            }


            return View(user);
        }

    }
}
