using System.ComponentModel.DataAnnotations;

namespace EchoHub.Models
{
    public class Category
    {
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Category name is required.")]
        [StringLength(50, ErrorMessage = "Category name cannot exceed 50 characters.")]
        [MinLength(3, ErrorMessage = "Category name must be at least 3 characters.")]
        public string Name { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;

    }
}
