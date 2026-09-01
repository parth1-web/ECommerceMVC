using System.Text.Json.Serialization;

namespace ECommerceMVC.Models.Api
{
    public class KhaltiInitiateResponseDto
    {
        [JsonPropertyName("orderId")]
        public int OrderId { get; set; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("pidx")]
        public string Pidx { get; set; } = string.Empty;

        [JsonPropertyName("paymentUrl")]
        public string PaymentUrl { get; set; } = string.Empty;
    }
}