using System.ComponentModel.DataAnnotations;

namespace EchoHub.Models
{
    public class EwasteItem
    {
        [Key]
        public int EwasteId { get; set; }

        [Required]
        public string Item_Name { get; set; }

        public string Category { get; set; }

        public string Description { get; set; }

        public string Image { get; set; }

        public string Status { get; set; }

        public decimal? AmountPaid { get; set; }

        public string? Address { get; set; }

        public DateTime DateSubmitted { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
