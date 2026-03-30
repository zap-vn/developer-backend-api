using MediatR;
using CRM.Order.Application.Orders.Queries.GetOrderList;

namespace CRM.Order.Application.Orders.Queries.GetOrderById
{
    public class GetOrderByIdQuery : IRequest<OrderListDto?>
    {
        public string Id { get; set; }

        public GetOrderByIdQuery(string id)
        {
            Id = id;
        }
    }
}
