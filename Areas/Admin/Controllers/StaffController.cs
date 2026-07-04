using HyliCoffeeWeb.Data;
using HyliCoffeeWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HyliCoffeeWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminOnly")]
    public class StaffController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StaffController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Staffs.OrderBy(s => s.Id).ToListAsync());
        }

        public IActionResult Create() => View(new Staff());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Staff staff)
        {
            if (!ModelState.IsValid) return View(staff);
            _context.Staffs.Add(staff);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Đã thêm nhân viên.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var staff = await _context.Staffs.FindAsync(id);
            if (staff == null) return NotFound();
            return View(staff);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Staff staff)
        {
            if (id != staff.Id) return NotFound();
            if (!ModelState.IsValid) return View(staff);

            _context.Update(staff);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Đã cập nhật nhân viên.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var staff = await _context.Staffs.FindAsync(id);
            if (staff == null) return NotFound();

            _context.Staffs.Remove(staff);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Đã xóa nhân viên.";
            return RedirectToAction(nameof(Index));
        }
    }
}
