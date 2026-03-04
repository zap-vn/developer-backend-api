using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using ZAP.Report.Application.Common.Interfaces;
using ZAP.Report.Application.Reports.DTOs;
using ZAP.Report.Infrastructure.Persistence;

namespace ZAP.Report.Infrastructure.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly IMongoCollection<BsonDocument> _orderCollection;

        public ReportRepository(MongoDbContext dbContext)
        {
            _orderCollection = dbContext.Database.GetCollection<BsonDocument>("Order");
        }

        public async Task<SalesSummaryDto> GetOverviewListLocationAsync(ReportRequestDto request, string userGuid)
        {
            var filter = Builders<BsonDocument>.Filter.Empty;

            var start = request.StartDay;
            var end = request.EndDay;
            var locationGuid = request.Location_id;

            if (string.IsNullOrEmpty(start)) start = DateTime.Now.ToString("yyyy-MM-dd 00:00:00");
            else start = DateTime.Parse(start).ToString("yyyy-MM-dd 00:00:00");

            if (string.IsNullOrEmpty(end)) end = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd 23:59:59");
            else end = DateTime.Parse(end).AddDays(1).ToString("yyyy-MM-dd 23:59:59");

            var builder = Builders<BsonDocument>.Filter;
            filter &= builder.Gte("CreateDate", start) & builder.Lte("CreateDate", end);

            filter &= builder.Eq("UserGuid", userGuid);
            filter &= builder.Eq("PaymentStatusId", 10); // Completed

            if (!string.IsNullOrEmpty(locationGuid))
            {
                filter &= builder.Or(
                    builder.Eq("AssignToLocationGuid", locationGuid),
                    builder.Eq("LocationGuid", locationGuid)
                );
            }

            if (request.IsAllDevices != true && request.DeviceIds != null && request.DeviceIds.Any())
            {
                filter &= builder.In("DeviceGuid", request.DeviceIds.Where(x => x != "null"));
            }

            var aggregate = _orderCollection.Aggregate()
                .Match(filter)
                .Lookup("OrderDetail", "_id", "Order_id", "details")
                .Unwind(new StringFieldDefinition<BsonDocument>("details"), new AggregateUnwindOptions<BsonDocument> { PreserveNullAndEmptyArrays = true })
                .Group(new BsonDocument
                {
                    { "_id", BsonNull.Value },
                    { "OrderQuantity", new BsonDocument("$sum", 1) },
                    { "OrderAmount", new BsonDocument("$sum", new BsonDocument("$ifNull", new BsonArray { "$details.Order.PaymentAmount", 0 })) },
                    { "GrossSale", new BsonDocument("$sum", new BsonDocument("$ifNull", new BsonArray { "$details.Order.SubTotal", 0 })) },
                    { "ServiceCharge", new BsonDocument("$sum", new BsonDocument("$ifNull", new BsonArray { "$details.Order.ServiceFee", 0 })) },
                    { "SurchargeFee", new BsonDocument("$sum", new BsonDocument("$ifNull", new BsonArray { "$details.Order.SurchargeFee", 0 })) },
                    { "TaxFee", new BsonDocument("$sum", new BsonDocument("$ifNull", new BsonArray { "$details.Order.TaxFee", 0 })) },
                    { "ShippingFee", new BsonDocument("$sum", new BsonDocument("$ifNull", new BsonArray { "$details.Order.ShippingFee", 0 })) },
                    { "OrderDiscount", new BsonDocument("$sum", new BsonDocument("$ifNull", new BsonArray { "$details.Order.Discount", 0 })) },
                    { "NetSales", new BsonDocument("$sum", new BsonDocument("$ifNull", new BsonArray { "$details.Order.NetSale", 0 })) },
                    { "TotalSales", new BsonDocument("$sum", new BsonDocument("$ifNull", new BsonArray { "$details.Order.Total", 0 })) },
                    { "OrderRefund", new BsonDocument("$sum", new BsonDocument("$cond", new BsonArray 
                        {
                            new BsonDocument("$eq", new BsonArray { "$isRefund", 1 }),
                            new BsonDocument("$ifNull", new BsonArray { "$details.Order.PaymentAmount", 0 }),
                            0
                        }) 
                    )},
                    { "QuantityRefund", new BsonDocument("$sum", new BsonDocument("$cond", new BsonArray 
                        {
                            new BsonDocument("$eq", new BsonArray { "$isRefund", 1 }),
                            1,
                            0
                        }) 
                    )}
                });

            var aggResult = await aggregate.FirstOrDefaultAsync();

            var summary = new SalesSummaryDto();
            if (aggResult != null)
            {
                summary.OrderQuantity = aggResult.GetValue("OrderQuantity", 0).AsInt32;
                summary.OrderAmount = aggResult.GetValue("OrderAmount", 0).IsDecimal128 ? (decimal)aggResult["OrderAmount"].AsDecimal128 : (decimal)aggResult["OrderAmount"].AsDouble;
                summary.GrossSale = aggResult.GetValue("GrossSale", 0).IsDecimal128 ? (decimal)aggResult["GrossSale"].AsDecimal128 : (decimal)aggResult["GrossSale"].AsDouble;
                summary.ServiceCharge = aggResult.GetValue("ServiceCharge", 0).IsDecimal128 ? (decimal)aggResult["ServiceCharge"].AsDecimal128 : (decimal)aggResult["ServiceCharge"].AsDouble;
                summary.ServiceFee = summary.ServiceCharge;
                summary.SurchargeFee = aggResult.GetValue("SurchargeFee", 0).IsDecimal128 ? (decimal)aggResult["SurchargeFee"].AsDecimal128 : (decimal)aggResult["SurchargeFee"].AsDouble;
                summary.TaxFee = aggResult.GetValue("TaxFee", 0).IsDecimal128 ? (decimal)aggResult["TaxFee"].AsDecimal128 : (decimal)aggResult["TaxFee"].AsDouble;
                summary.ShippingFee = aggResult.GetValue("ShippingFee", 0).IsDecimal128 ? (decimal)aggResult["ShippingFee"].AsDecimal128 : (decimal)aggResult["ShippingFee"].AsDouble;
                summary.OrderDiscount = aggResult.GetValue("OrderDiscount", 0).IsDecimal128 ? (decimal)aggResult["OrderDiscount"].AsDecimal128 : (decimal)aggResult["OrderDiscount"].AsDouble;
                summary.NetSales = aggResult.GetValue("NetSales", 0).IsDecimal128 ? (decimal)aggResult["NetSales"].AsDecimal128 : (decimal)aggResult["NetSales"].AsDouble;
                summary.TotalSales = aggResult.GetValue("TotalSales", 0).IsDecimal128 ? (decimal)aggResult["TotalSales"].AsDecimal128 : (decimal)aggResult["TotalSales"].AsDouble;
                summary.OrderRefund = aggResult.GetValue("OrderRefund", 0).IsDecimal128 ? (decimal)aggResult["OrderRefund"].AsDecimal128 : (decimal)aggResult["OrderRefund"].AsDouble;
                var refundQty = aggResult.GetValue("QuantityRefund", 0);
                summary.QuantityRefund = refundQty.IsDecimal128 ? (decimal)refundQty.AsDecimal128 : refundQty.IsDouble ? (decimal)refundQty.AsDouble : (decimal)refundQty.AsInt32;
                
                // Average calculations
                summary.BillCount = summary.OrderQuantity;
                if (summary.BillCount > 0)
                {
                    summary.BillAverage = summary.TotalSales / (decimal)summary.BillCount;
                    summary.BillAverageGrossSale = summary.GrossSale / (decimal)summary.BillCount;
                    summary.BillAverageNetSale = summary.NetSales / (decimal)summary.BillCount;
                }
            }

            return summary;
        }
    }
}
