namespace HyliCoffeeWeb.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }
        public int TotalUsers { get; set; }
        public int TotalArticles { get; set; }
        public decimal TotalRevenue { get; set; }
        public int PendingOrders { get; set; }
        public List<HyliCoffeeWeb.Models.Order> RecentOrders { get; set; } = new();
        public List<HyliCoffeeWeb.Models.Feedback> RecentFeedbacks { get; set; } = new();
    }
}
