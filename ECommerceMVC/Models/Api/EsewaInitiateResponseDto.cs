
using System.Text.Json.Serialization;

namespace ECommerceMVC.Models.Api
{
    public class EsewaInitiateResponseDto
    {
        [JsonPropertyName("orderId")]
        public int OrderId { get; set; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("transactionUuid")]
        public string TransactionUuid { get; set; } = string.Empty;

        [JsonPropertyName("paymentUrl")]
        public string PaymentUrl { get; set; } = string.Empty;

        [JsonPropertyName("formData")]
        public EsewaFormDataDto FormData { get; set; } = new();
    }

    public class EsewaFormDataDto
    {
        [JsonPropertyName("amount")]
        public string Amount { get; set; } = string.Empty;

        [JsonPropertyName("tax_amount")]
        public string TaxAmount { get; set; } = string.Empty;

        [JsonPropertyName("total_amount")]
        public string TotalAmount { get; set; } = string.Empty;

        [JsonPropertyName("transaction_uuid")]
        public string TransactionUuid { get; set; } = string.Empty;

        [JsonPropertyName("product_code")]
        public string ProductCode { get; set; } = string.Empty;

        [JsonPropertyName("product_service_charge")]
        public string ProductServiceCharge { get; set; } = string.Empty;

        [JsonPropertyName("product_delivery_charge")]
        public string ProductDeliveryCharge { get; set; } = string.Empty;

        [JsonPropertyName("success_url")]
        public string SuccessUrl { get; set; } = string.Empty;

        [JsonPropertyName("failure_url")]
        public string FailureUrl { get; set; } = string.Empty;

        [JsonPropertyName("signed_field_names")]
        public string SignedFieldNames { get; set; } = string.Empty;

        [JsonPropertyName("signature")]
        public string Signature { get; set; } = string.Empty;
    }
}
