using System.Text.Json.Serialization;

namespace ECommerceMVC.Models.Api;

public class OrderDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }


    [JsonPropertyName("orderDate")]
    public DateTime OrderDate { get; set; }


    [JsonPropertyName("status")]
    public int Status { get; set; }


    [JsonPropertyName("totalAmount")]
    public decimal TotalAmount { get; set; }


    [JsonPropertyName("payment")]
    public PaymentDto? Payment { get; set; }


    [JsonPropertyName("items")]
    public List<OrderItemDto> Items { get; set; } = new();
}