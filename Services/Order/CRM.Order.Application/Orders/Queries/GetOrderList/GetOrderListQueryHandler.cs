using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CRM.BuildingBlocks.Models;
using CRM.Order.Domain.Interfaces;
using CRM.Order.Domain.Entities;
using MediatR;

namespace CRM.Order.Application.Orders.Queries.GetOrderList
{
    public class GetOrderListQueryHandler : IRequestHandler<GetOrderListQuery, PagedResult<OrderListDto>>
    {
        private readonly IOrderRepository _orderRepository;

        public GetOrderListQueryHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<PagedResult<OrderListDto>> Handle(GetOrderListQuery request, CancellationToken cancellationToken)
        {
            var filter    = request.Filter ?? new FilterDTOs();
            int pageIndex = filter.PageIndex > 0 ? filter.PageIndex : 1;
            int pageSize  = filter.PageSize  > 0 ? filter.PageSize  : 20;

            var (items, totalCount) = await _orderRepository.GetOrderListAsync(
                keyword:   filter.Keyword,
                status:    filter.Status,
                pageIndex: pageIndex,
                pageSize:  pageSize);

            // Fetch related OrderDetails and Locations in bulk
            var orderIds = items.Select(x => x.Id).ToList();
            var locationGuids = items.Select(x => !string.IsNullOrEmpty(x.AssignToLocationGuid) ? x.AssignToLocationGuid : x.LocationGuid)
                                     .Where(x => !string.IsNullOrEmpty(x))
                                     .Cast<string>()
                                     .Distinct().ToList();

            var orderDetails = await _orderRepository.GetRelatedOrderDetailsAsync(orderIds);
            var locations = await _orderRepository.GetLocationsAsync(locationGuids);

            var detailDict = orderDetails.ToDictionary(x => x.OrderId, x => x);
            var locationDict = locations.ToDictionary(x => x.Id, x => x);

            var dtos = items.Select(o => 
            {
                var effectiveLocationGuid = !string.IsNullOrEmpty(o.AssignToLocationGuid) ? o.AssignToLocationGuid : o.LocationGuid;

                var dto = new OrderListDto
                {
                    Id                  = o.Id              ?? string.Empty,
                    Key                 = o.Key,
                    OrderCode           = o.OrderCode,
                    Code                = o.OrderCode,
                    CartId              = o.CartId          ?? string.Empty,
                    OrderStatusId       = o.OrderStatusId,
                    PaymentStatusId     = o.PaymentStatusId,
                    DiningOptionId      = o.DiningOptionId,
                    LocationGuid        = o.LocationGuid    ?? string.Empty,
                    DeviceGuid          = o.DeviceGuid      ?? string.Empty,
                    UserGuid            = o.UserGuid        ?? string.Empty,
                    CustomerGuestGuid   = o.CustomerGuestGuid ?? string.Empty,
                    AssignToLocationGuid = effectiveLocationGuid ?? string.Empty,
                    TotalAmount         = o.TotalAmount,
                    Status              = o.Status,
                    ItemCount           = o.Items?.Count    ?? 0,
                    CreatedAt           = o.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                    UpdatedAt           = o.UpdatedAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? string.Empty
                };

                // Map data from OrderDetail if found
                if (o.Id != null && detailDict.TryGetValue(o.Id, out var detail))
                {
                    dto.Subtotal    = detail.OrderSummary.Subtotal;
                    dto.Discount    = detail.OrderSummary.Discount;
                    dto.Grandtotal   = detail.OrderSummary.Grandtotal;
                    dto.Shipping    = detail.Shipping;
                    dto.PaymentList = detail.PaymentList;
                }

                // Map Location Name if found
                if (!string.IsNullOrEmpty(effectiveLocationGuid))
                {
                    if (locationDict.TryGetValue(effectiveLocationGuid, out var location))
                    {
                        dto.AssignToLocationName = location.Name;
                    }
                    else
                    {
                        dto.AssignToLocationName = "Name not found for: " + effectiveLocationGuid;
                    }
                }

                return dto;
            }).ToList();

            return new PagedResult<OrderListDto>(dtos, (int)totalCount, pageIndex, pageSize);
        }
    }
}
