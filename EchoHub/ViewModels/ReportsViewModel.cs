using EchoHub.Models;

namespace EchoHub.ViewModels
{
    public class ReportsViewModel
    {
        //SUMARRY
        public int TotalSubmitted { get; set; }
        public int TotalRecycled { get; set; }
        public int TotalDisposed { get; set; }

        //USER ACTIVTY 
        public int ActiveUsers { get; set; }
        public int NewUsers { get; set; }

        //TABLE DATA
        public List<MonthlyStats> monthlystats { get; set; } = new();
        public List<CategoryStats> categorystats { get; set; } = new();
    }

    public class MonthlyStats
    { 
        public string MonthName { get; set; }
        public int Total { get; set; }
        public int Recycled { get; set; }
        public int Disposed { get; set; }
    }

    public class CategoryStats
    {
        public string Category { get; set; }
        public int Count { get; set; }

        public double Percentage { get; set; }
    }
}
