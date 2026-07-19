using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HyliCoffeeWeb.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm")]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
        public decimal Price { get; set; }

        // VD: "Tranh meme", "Trà", "Cà phê"...
        [StringLength(100)]
        public string? Type { get; set; }

        // Tên file ảnh lưu trong wwwroot/img (giữ nguyên cấu trúc ảnh gốc của template)
        [StringLength(300)]
        public string? Image { get; set; }

        public string? Description { get; set; }

        // Hiển thị ở khu vực "Online Store" trang chủ
        public bool Featured { get; set; } = false;

        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
