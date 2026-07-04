using HyliCoffeeWeb.Data;
using HyliCoffeeWeb.Models;
using HyliCoffeeWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HyliCoffeeWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminOnly")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET /Admin (Admin/Dashboard/Index) -> tương ứng admin/index.html
        public async Task<IActionResult> Index()
        {
            var vm = new DashboardViewModel
            {
                TotalProducts = await _context.Products.CountAsync(),
                TotalOrders = await _context.Orders.CountAsync(),
                TotalUsers = await _context.Users.CountAsync(u => u.Role == UserRole.Customer),
                TotalArticles = await _context.Articles.CountAsync(),
                TotalRevenue = await _context.Orders
                    .Where(o => o.Status == OrderStatus.HoanThanh)
                    .SumAsync(o => (decimal?)o.Total) ?? 0,
                PendingOrders = await _context.Orders.CountAsync(o => o.Status == OrderStatus.ChoXuLy),
                RecentOrders = await _context.Orders
                    .Include(o => o.User)
                    .OrderByDescending(o => o.OrderDate)
                    .Take(5)
                    .ToListAsync(),
                RecentFeedbacks = await _context.Feedbacks
                    .OrderByDescending(f => f.CreatedDate)
                    .Take(5)
                    .ToListAsync()
            };

            return View(vm);
        }
    }
}
