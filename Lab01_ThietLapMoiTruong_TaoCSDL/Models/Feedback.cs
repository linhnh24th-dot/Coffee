using System.ComponentModel.DataAnnotations;

namespace HyliCoffeeWeb.Models
{
    public class Feedback
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required, StringLength(150), EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
