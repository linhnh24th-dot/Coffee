using System.ComponentModel.DataAnnotations;

namespace HyliCoffeeWeb.Models
{
    public class Article
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(250)]
        public string Title { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Category { get; set; }

        [StringLength(300)]
        public string? Image { get; set; }

        public string? Content { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
