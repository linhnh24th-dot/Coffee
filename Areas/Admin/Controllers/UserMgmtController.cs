using HyliCoffeeWeb.Data;
using HyliCoffeeWeb.Models;
using HyliCoffeeWeb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HyliCoffeeWeb.Areas.Admin.Controllers
{
    // Đặt tên UserMgmt để tránh trùng với AccountController (đăng nhập/đăng ký phía khách)
    [Area("Admin")]
    [Authorize(Policy = "AdminOnly")]
    public class UserMgmtController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher _passwordHasher;

        public UserMgmtController(ApplicationDbContext context, IPasswordHasher passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.OrderBy(u => u.Id).ToListAsync());
        }

        public IActionResult Create() => View(new User());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user, string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                ModelState.AddModelError(nameof(password), "Vui lòng nhập mật khẩu");

            if (!ModelState.IsValid) return View(user);

            bool exists = await _context.Users.AnyAsync(u => u.Username == user.Username);
            if (exists)
            {
                ModelState.AddModelError(nameof(user.Username), "Tên đăng nhập đã tồn tại.");
                return View(user);
            }

            user.Password = _passwordHasher.Hash(password);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đã thêm tài khoản.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, User user, string? newPassword)
        {
            if (id != user.Id) return NotFound();

            var existing = await _context.Users.FindAsync(id);
            if (existing == null) return NotFound();

            existing.FullName = user.FullName;
            existing.Phone = user.Phone;
            existing.Email = user.Email;
            existing.Role = user.Role;

            if (!string.IsNullOrWhiteSpace(newPassword))
            {
                existing.Password = _passwordHasher.Hash(newPassword);
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Đã cập nhật tài khoản.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Đã xóa tài khoản.";
            return RedirectToAction(nameof(Index));
        }
    }
}
