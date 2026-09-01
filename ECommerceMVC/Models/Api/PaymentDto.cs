using System.Text.Json.Serialization;

namespace ECommerceMVC.Models.Api
{
    public class PaymentDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("orderId")]
        public int OrderId { get; set; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("method")]
        public int Method { get; set; }

        [JsonPropertyName("status")]
        public int Status { get; set; }

        [JsonPropertyName("transactionId")]
        public string? TransactionId { get; set; }

        [JsonPropertyName("paymentDate")]
        public DateTime PaymentDate { get; set; }
    }
}