using HyliCoffeeWeb.Data;
using HyliCoffeeWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace HyliCoffeeWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var featuredProducts = await _context.Products
                .Where(p => p.Featured)
                .OrderByDescending(p => p.Id)
                .Take(6)
                .ToListAsync();

            var latestArticles = await _context.Articles
                .OrderByDescending(a => a.CreatedDate)
                .Take(3)
                .ToListAsync();

            ViewBag.FeaturedProducts = featuredProducts;
            ViewBag.LatestArticles   = latestArticles;
            return View();
        }

        public IActionResult News()
        {
            // Lấy danh sách tin tức từ database, xếp tin mới nhất lên đầu
            var articles = _context.Articles.OrderByDescending(a => a.CreatedDate).ToList();
            return View(articles);
        }

        [HttpGet]
        public IActionResult Contact()
        {
            return View(new Feedback());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(Feedback feedback)
        {
            if (!ModelState.IsValid)
            {
                return View(feedback);
            }

            feedback.CreatedDate = DateTime.Now;
            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Cảm ơn bạn đã liên hệ! Chúng tôi sẽ phản hồi sớm nhất.";
            return RedirectToAction(nameof(Contact));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        // FIX LỖI 8: Thêm tham số code để nhận HTTP status code từ UseStatusCodePagesWithReExecute
        // VD: /Home/Error?code=404 → hiển thị "Trang không tồn tại"
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int? code)
        {
            ViewBag.StatusCode = code;
            ViewBag.RequestId  = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            return View();
        }
    }
}
