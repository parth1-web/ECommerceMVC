using System.Text.Json.Serialization;

namespace ECommerceMVC.Models.Api;

public class OrderSummaryDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("orderDate")]
    public DateTime OrderDate { get; set; }

    [JsonPropertyName("status")]
    public int Status { get; set; }

    [JsonPropertyName("totalAmount")]
    public decimal TotalAmount { get; set; }

    [JsonPropertyName("itemCount")]
    public int ItemCount { get; set; }
}