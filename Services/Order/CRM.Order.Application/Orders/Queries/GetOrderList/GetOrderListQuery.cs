using CRM.BuildingBlocks.Models;
using MediatR;

namespace CRM.Order.Application.Orders.Queries.GetOrderList
{
    public class GetOrderListQuery : IRequest<PagedResult<OrderListDto>>
    {
        public FilterDTOs Filter { get; set; } = new();
    }
}
