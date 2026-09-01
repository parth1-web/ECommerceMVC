namespace ECommerceMVC.Models
{
    public class ProductViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int Stock { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        public bool IsAvailable => Stock > 0;
    }
}