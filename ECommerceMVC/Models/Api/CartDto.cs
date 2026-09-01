namespace ECommerceMVC.Models.Api
{
    public class CartDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public List<CartItemDto> Items { get; set; } = new();

        public decimal TotalAmount { get; set; }

    }
}