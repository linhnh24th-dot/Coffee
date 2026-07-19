using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HyliCoffeeWeb.Models
{
    public enum OrderStatus
    {
        ChoXuLy = 0,      // Chờ xử lý
        DangGiao = 1,     // Đang giao
        HoanThanh = 2,    // Hoàn thành
        DaHuy = 3         // Đã hủy
    }

    public class Order
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }

        [Required, StringLength(150)]
        public string Receiver { get; set; } = string.Empty;

        [Required, StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        [Required, StringLength(300)]
        public string Address { get; set; } = string.Empty;

        public DateTime OrderDate { get; set; } = DateTime.Now;

        public OrderStatus Status { get; set; } = OrderStatus.ChoXuLy;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
