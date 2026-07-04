using System.ComponentModel.DataAnnotations;

namespace HyliCoffeeWeb.Models
{
    public enum UserRole
    {
        Customer = 0,
        Admin = 1
    }

    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Username { get; set; } = string.Empty;

        // Lưu mật khẩu đã hash (BCrypt/PBKDF2), KHÔNG lưu plaintext
        [Required]
        public string Password { get; set; } = string.Empty;

        [StringLength(150)]
        public string? FullName { get; set; }

        [StringLength(20)]
        public string? Phone { get; set; }

        [StringLength(150)]
        public string? Email { get; set; }

        public UserRole Role { get; set; } = UserRole.Customer;

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
