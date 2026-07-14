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
            // --- PHẦN THÊM MỚI: Tính toán doanh thu theo từng tháng của năm hiện tại ---
            int currentYear = DateTime.Now.Year;

            var monthlyData = await _context.Orders
                .Where(o => o.OrderDate.Year == currentYear && o.Status == OrderStatus.HoanThanh)
                .GroupBy(o => o.OrderDate.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    Total = g.Sum(o => o.Total)
                })
                .ToListAsync();

            // Khởi tạo mảng 12 phần tử tương ứng 12 tháng (mặc định bằng 0)
            decimal[] revenueArray = new decimal[12];
            foreach (var item in monthlyData)
            {
                revenueArray[item.Month - 1] = item.Total;
            }

            // Gửi dữ liệu sang View bằng ViewBag
            ViewBag.MonthlyRevenue = revenueArray;
            ViewBag.CurrentYear = currentYear;
            // -------------------------------------------------------------------------

            // Giữ nguyên ViewModel cũ của bạn
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