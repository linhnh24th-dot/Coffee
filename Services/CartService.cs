using HyliCoffeeWeb.Models;
using System.Text.Json;

namespace HyliCoffeeWeb.Services
{
    public class CartItem
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? Image { get; set; }
        public int Quantity { get; set; }
        public decimal Total => Price * Quantity;
    }

    /// <summary>
    /// Giỏ hàng được lưu trong Session (server-side) thay cho localStorage ở bản JS gốc.
    /// Khi checkout sẽ ghi xuống bảng Order/OrderDetail qua EF Core.
    /// </summary>
    public interface ICartService
    {
        List<CartItem> GetCart(ISession session);
        void AddToCart(ISession session, Product product, int quantity);
        void UpdateQuantity(ISession session, int productId, int quantity);
        void RemoveItem(ISession session, int productId);
        void ClearCart(ISession session);
        decimal GetTotal(ISession session);
    }

    public class CartService : ICartService
    {
        private const string CartSessionKey = "Cart";

        public List<CartItem> GetCart(ISession session)
        {
            var json = session.GetString(CartSessionKey);
            if (string.IsNullOrEmpty(json)) return new List<CartItem>();
            return JsonSerializer.Deserialize<List<CartItem>>(json) ?? new List<CartItem>();
        }

        private void SaveCart(ISession session, List<CartItem> cart)
        {
            session.SetString(CartSessionKey, JsonSerializer.Serialize(cart));
        }

        public void AddToCart(ISession session, Product product, int quantity)
        {
            var cart = GetCart(session);
            var existing = cart.FirstOrDefault(c => c.ProductId == product.Id);
            if (existing != null)
            {
                existing.Quantity += quantity;
            }
            else
            {
                cart.Add(new CartItem
                {
                    ProductId = product.Id,
                    Name = product.Name,
                    Price = product.Price,
                    Image = product.Image,
                    Quantity = quantity
                });
            }
            SaveCart(session, cart);
        }

        public void UpdateQuantity(ISession session, int productId, int quantity)
        {
            var cart = GetCart(session);
            var item = cart.FirstOrDefault(c => c.ProductId == productId);
            if (item != null)
            {
                item.Quantity = Math.Max(1, quantity);
                SaveCart(session, cart);
            }
        }

        public void RemoveItem(ISession session, int productId)
        {
            var cart = GetCart(session);
            cart.RemoveAll(c => c.ProductId == productId);
            SaveCart(session, cart);
        }

        public void ClearCart(ISession session)
        {
            session.Remove(CartSessionKey);
        }

        public decimal GetTotal(ISession session)
        {
            return GetCart(session).Sum(c => c.Total);
        }
    }
}
