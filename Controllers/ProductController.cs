using HyliCoffeeWeb.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HyliCoffeeWeb.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET /Product?keyword=&type=
        // Thay thế hoàn toàn việc đọc productList từ localStorage trong product.html gốc
        public async Task<IActionResult> Index(string? keyword, string? type)
        {
            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(p => p.Name.Contains(keyword));
            }

            if (!string.IsNullOrWhiteSpace(type))
            {
                query = query.Where(p => p.Type == type);
            }

            ViewBag.Keyword = keyword;
            ViewBag.Type = type;
            ViewBag.Types = await _context.Products
                .Select(p => p.Type)
                .Where(t => t != null)
                .Distinct()
                .ToListAsync();

            var products = await query.OrderByDescending(p => p.Id).ToListAsync();
            return View(products);
        }

        // GET /Product/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();
            return View(product);
        }

        // GET /Product/Store -> tương ứng store.html (showroom tranh meme)
        public async Task<IActionResult> Store()
        {
            var products = await _context.Products
                .Where(p => p.Type == "Tranh meme")
                .ToListAsync();
            return View(products);
        }
    }
}
