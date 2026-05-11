using EchoHub.Data;
using EchoHub.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EchoHub.Controllers
{
    public class ReportsController : Controller
    {
        private readonly AppDbContext _context;

        public ReportsController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Reports() 
        {
            var model = new ReportsViewModel();

            //SUMMARY OF THE TOTAL SUBMITTED, RECYCLED, AND DISPOSED ITEMS
            model.TotalSubmitted = _context.EwasteItems.Count();
            model.TotalRecycled = _context.EwasteItems.Count(e => e.Status == "Recycled");
            model.TotalDisposed = _context.EwasteItems.Count(e => e.Status == "Disposed");

            //USER ACTIVTY STATS IT SHOWS THE TOTAL NUMBER OF ACTIVE USERS AND THE NUMBER OF NEW USERS IN THE CURRENT MONTH
            model.ActiveUsers = _context.Users.Count();
            model.NewUsers = _context.Users
                .Where(u => u.CreatedAt.Month == DateTime.Now.Month & u.CreatedAt.Year == DateTime.Now.Year).Count();

            //MONTHLY STATS IT SHOWS THE NUMBER OF ITEMS SUBMITTED, RECYCLED, AND DISPOSED FOR EACH MONTH. THIS HELPS TO IDENTIFY TRENDS IN E-WASTE MANAGEMENT OVER TIME.
            model.monthlystats = _context.EwasteItems
                 .GroupBy(e => new
                 {
                     e.DateSubmitted.Year,
                     e.DateSubmitted.Month
                 })
                 .AsEnumerable()
                 .Select(g => new MonthlyStats
                 {
                     MonthName = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMMM yyyy"),

                     Total = g.Count(),

                     Recycled = g.Count(e => e.Status == "Recycled"),

                     Disposed = g.Count(e => e.Status == "Disposed")
                 }). OrderByDescending(e => e.MonthName).ToList();

            //CATEGORY BREAKDOWN IT SHOWS THE NUMBER OF ITEMS IN EACH CATEGORY AND THEIR PERCENTAGE OF THE TOTAL SUBMISSIONS. THIS HELPS TO IDENTIFY WHICH TYPES OF E-WASTE ARE MOST COMMON AND MAY REQUIRE MORE FOCUS IN RECYCLING EFFORTS.
            var TotalItems = _context.EwasteItems.Count();

            model.categorystats = _context.EwasteItems
                .GroupBy(e => e.Category)
                .Select(c => new CategoryStats
                {
                    Category = c.Key,
                    Count = c.Count(),
                    Percentage = TotalItems == 0 ? 0 : (double)c.Count() / TotalItems * 100
                })
                .OrderByDescending(c => c.Count)
                .ToList();  

            return View(model);
        }
    }
}
