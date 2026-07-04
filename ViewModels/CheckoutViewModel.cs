using System.ComponentModel.DataAnnotations;
using HyliCoffeeWeb.Services;

namespace HyliCoffeeWeb.ViewModels
{
    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên người nhận")]
        public string Receiver { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ giao hàng")]
        public string Address { get; set; } = string.Empty;

        public List<CartItem> CartItems { get; set; } = new();

        public decimal Total => CartItems.Sum(c => c.Total);
    }
}
