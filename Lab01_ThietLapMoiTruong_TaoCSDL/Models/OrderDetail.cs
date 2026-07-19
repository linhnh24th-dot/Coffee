using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HyliCoffeeWeb.Models
{
    public class OrderDetail
    {
        [Key]
        public int Id { get; set; }

        public int OrderId { get; set; }
        [ForeignKey(nameof(OrderId))]
        public Order? Order { get; set; }

        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product? Product { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        // Giá tại thời điểm đặt hàng (chốt giá, không bị ảnh hưởng nếu Product.Price đổi sau này)
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
    }
}
