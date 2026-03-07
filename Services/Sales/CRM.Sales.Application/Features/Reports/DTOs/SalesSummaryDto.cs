using System.Text.Json.Serialization;

namespace CRM.Sales.Application.Features.Reports.DTOs
{
    public class SalesSummaryDto
    {
        [JsonPropertyName("OrderQuantity")]
        public int? OrderQuantity { get; set; }

        [JsonPropertyName("Cover")]
        public decimal? Cover { get; set; }

        [JsonPropertyName("OrderAmount")]
        public decimal? OrderAmount { get; set; }

        [JsonPropertyName("ServiceCharge")]
        public decimal? ServiceCharge { get; set; }

        [JsonPropertyName("Comp")]
        public decimal? Comp { get; set; }

        [JsonPropertyName("OrderDiscount")]
        public decimal? OrderDiscount { get; set; }

        [JsonPropertyName("OrderDiscountComp")]
        public decimal? OrderDiscountComp { get; set; }

        [JsonPropertyName("GrossSale")]
        public decimal? GrossSale { get; set; }

        [JsonPropertyName("ServiceFee")]
        public decimal? ServiceFee { get; set; }

        [JsonPropertyName("SurchargeFee")]
        public decimal? SurchargeFee { get; set; }

        [JsonPropertyName("TaxFee")]
        public decimal? TaxFee { get; set; }

        [JsonPropertyName("ShippingFee")]
        public decimal? ShippingFee { get; set; }

        [JsonPropertyName("NetSales")]
        public decimal? NetSales { get; set; }

        [JsonPropertyName("TotalSales")]
        public decimal? TotalSales { get; set; }

        [JsonPropertyName("QuantityRefund")]
        public decimal? QuantityRefund { get; set; }

        [JsonPropertyName("OrderRefund")]
        public decimal? OrderRefund { get; set; }

        [JsonPropertyName("QuantityBillCount")]
        public decimal? QuantityBillCount { get; set; }

        [JsonPropertyName("BillCount")]
        public decimal? BillCount { get; set; }

        [JsonPropertyName("BillAverage")]
        public decimal? BillAverage { get; set; }

        [JsonPropertyName("BillCountGrossSale")]
        public decimal? BillCountGrossSale { get; set; }

        [JsonPropertyName("BillAverageGrossSale")]
        public decimal? BillAverageGrossSale { get; set; }

        [JsonPropertyName("BillCountNetSale")]
        public decimal? BillCountNetSale { get; set; }

        [JsonPropertyName("BillAverageNetSale")]
        public decimal? BillAverageNetSale { get; set; }
    }
}
