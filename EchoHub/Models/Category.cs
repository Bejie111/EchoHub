namespace EchoHub.Models
{
    public class Category
    {
        public int CategoryId { get; set; }

        public string Name { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.Now;

    }
}
