namespace ZAP.Report.Application.Reports.DTOs
{
    public class SalesSummaryDto
    {
        public int? OrderQuantity { get; set; }
        public decimal? Cover { get; set; }
        public decimal? OrderAmount { get; set; }
        public decimal? ServiceCharge { get; set; }
        public decimal? Comp { get; set; }
        public decimal? OrderDiscount { get; set; }
        public decimal? OrderDiscountComp { get; set; }
        public decimal? GrossSale { get; set; }
        public decimal? ServiceFee { get; set; }
        public decimal? SurchargeFee { get; set; }
        public decimal? TaxFee { get; set; }
        public decimal? ShippingFee { get; set; }
        public decimal? NetSales { get; set; }
        public decimal? TotalSales { get; set; }
        public decimal? QuantityRefund { get; set; }
        public decimal? OrderRefund { get; set; }
        public decimal? QuantityBillCount { get; set; }
        public decimal? BillCount { get; set; }
        public decimal? BillAverage { get; set; }
        public decimal? BillCountGrossSale { get; set; }
        public decimal? BillAverageGrossSale { get; set; }
        public decimal? BillCountNetSale { get; set; }
        public decimal? BillAverageNetSale { get; set; }
    }
}
