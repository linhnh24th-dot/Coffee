using HyliCoffeeWeb.Data;
using HyliCoffeeWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HyliCoffeeWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminOnly")]
    public class ArticleController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ArticleController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var articles = await _context.Articles.OrderByDescending(a => a.CreatedDate).ToListAsync();
            return View(articles);
        }

        public IActionResult Create() => View(new Article());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Article article, IFormFile? imageFile)
        {
            if (!ModelState.IsValid) return View(article);

            if (imageFile != null && imageFile.Length > 0)
                article.Image = await SaveImageAsync(imageFile);

            article.CreatedDate = DateTime.Now;
            _context.Articles.Add(article);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đã thêm tin tức.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article == null) return NotFound();
            return View(article);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Article article, IFormFile? imageFile)
        {
            if (id != article.Id) return NotFound();
            if (!ModelState.IsValid) return View(article);

            var existing = await _context.Articles.FindAsync(id);
            if (existing == null) return NotFound();

            existing.Title = article.Title;
            existing.Category = article.Category;
            existing.Content = article.Content;

            if (imageFile != null && imageFile.Length > 0)
                existing.Image = await SaveImageAsync(imageFile);

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Đã cập nhật tin tức.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article == null) return NotFound();

            _context.Articles.Remove(article);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đã xóa tin tức.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            var uploadsFolder = Path.Combine(_env.WebRootPath, "img", "uploads");
            Directory.CreateDirectory(uploadsFolder);
            var fileName = Guid.NewGuid().ToString("N") + Path.GetExtension(imageFile.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);
            using var stream = new FileStream(filePath, FileMode.Create);
            await imageFile.CopyToAsync(stream);
            return "uploads/" + fileName;
        }
    }
}
