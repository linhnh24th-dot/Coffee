using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HyliCoffeeWeb.Models
{
    public class Staff
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Phone { get; set; }

        // VD: "Ca sáng", "Ca chiều", "Ca tối"
        [StringLength(50)]
        public string? Shift { get; set; }

        [StringLength(100)]
        public string? Position { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Salary { get; set; }
    }
}
