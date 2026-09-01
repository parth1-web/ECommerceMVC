using System.Text.Json.Serialization;

namespace ECommerceMVC.Models.Api
{
    public class PaymentRequestDto
    {
        [JsonPropertyName("orderId")]
        public int OrderId { get; set; }

        [JsonPropertyName("method")]
        public int Method { get; set; }
    }
}