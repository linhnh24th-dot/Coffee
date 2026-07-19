using HyliCoffeeWeb.Data;
using HyliCoffeeWeb.Models;
using HyliCoffeeWeb.Services;
using HyliCoffeeWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HyliCoffeeWeb.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICartService _cartService;

        public CartController(ApplicationDbContext context, ICartService cartService)
        {
            _context = context;
            _cartService = cartService;
        }

        // GET /Cart -> hiển thị _CartModal.cshtml bằng dữ liệu Session
        public IActionResult Index()
        {
            var cart = _cartService.GetCart(HttpContext.Session);
            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> Add(int productId, int quantity = 1)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return NotFound();

            _cartService.AddToCart(HttpContext.Session, product, quantity);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            _cartService.UpdateQuantity(HttpContext.Session, productId, quantity);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Remove(int productId)
        {
            _cartService.RemoveItem(HttpContext.Session, productId);
            return RedirectToAction(nameof(Index));
        }

        // ===== Thanh toán: bắt buộc đăng nhập =====
        [Authorize]
        [HttpGet]
        public IActionResult Checkout()
        {
            var cart = _cartService.GetCart(HttpContext.Session);
            if (cart.Count == 0)
            {
                TempData["ErrorMessage"] = "Giỏ hàng của bạn đang trống.";
                return RedirectToAction(nameof(Index));
            }

            return View(new CheckoutViewModel { CartItems = cart });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            var cart = _cartService.GetCart(HttpContext.Session);
            model.CartItems = cart;

            if (cart.Count == 0)
            {
                TempData["ErrorMessage"] = "Giỏ hàng của bạn đang trống.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var order = new Order
            {
                UserId = userId,
                Receiver = model.Receiver,
                Phone = model.Phone,
                Address = model.Address,
                OrderDate = DateTime.Now,
                Status = OrderStatus.ChoXuLy,
                Total = cart.Sum(c => c.Total)
            };

            foreach (var item in cart)
            {
                order.OrderDetails.Add(new OrderDetail
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price
                });
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            _cartService.ClearCart(HttpContext.Session);

            TempData["SuccessMessage"] = "Đặt hàng thành công! Mã đơn hàng của bạn: #" + order.Id;
            return RedirectToAction("History", "Order", new { area = "" });
        }
    }
}
