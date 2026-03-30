using MediatR;
using CRM.Order.Application.Orders.Queries.GetOrderList;
using CRM.Order.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace CRM.Order.Application.Orders.Queries.GetOrderById
{
    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderListDto?>
    {
        private readonly IOrderRepository _orderRepository;

        public GetOrderByIdQueryHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<OrderListDto?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Id)) return null;

            var order = await _orderRepository.GetByIdAsync(request.Id);
            if (order == null) return null;

            // Fetch related details
            var orderDetails = await _orderRepository.GetRelatedOrderDetailsAsync(new[] { order.Id });
            var detail = orderDetails.FirstOrDefault();

            var effectiveLocationGuid = !string.IsNullOrEmpty(order.AssignToLocationGuid) ? order.AssignToLocationGuid : order.LocationGuid;
            var locationName = string.Empty;
            if (!string.IsNullOrEmpty(effectiveLocationGuid))
            {
                var locations = await _orderRepository.GetLocationsAsync(new[] { effectiveLocationGuid });
                var location = locations.FirstOrDefault();
                locationName = location?.Name ?? "Name not found for: " + effectiveLocationGuid;
            }

            var dto = new OrderListDto
            {
                Id                  = order.Id              ?? string.Empty,
                Key                 = order.Key,
                OrderCode           = order.OrderCode,
                Code                = order.OrderCode,
                CartId              = order.CartId          ?? string.Empty,
                OrderStatusId       = order.OrderStatusId,
                PaymentStatusId     = order.PaymentStatusId,
                DiningOptionId      = order.DiningOptionId,
                LocationGuid        = order.LocationGuid    ?? string.Empty,
                DeviceGuid          = order.DeviceGuid      ?? string.Empty,
                UserGuid            = order.UserGuid        ?? string.Empty,
                CustomerGuestGuid   = order.CustomerGuestGuid ?? string.Empty,
                AssignToLocationGuid = effectiveLocationGuid ?? string.Empty,
                AssignToLocationName = locationName,
                TotalAmount         = order.TotalAmount,
                Status              = order.Status,
                ItemCount           = order.Items?.Count    ?? 0,
                CreatedAt           = order.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                UpdatedAt           = order.UpdatedAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? string.Empty
            };

            if (detail != null)
            {
                dto.Subtotal    = detail.OrderSummary.Subtotal;
                dto.Discount    = detail.OrderSummary.Discount;
                dto.Grandtotal   = detail.OrderSummary.Grandtotal;
                dto.Shipping    = detail.Shipping;
                dto.PaymentList = detail.PaymentList;
            }

            return dto;
        }
    }
}
