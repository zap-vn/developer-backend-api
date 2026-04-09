using MediatR;
using CRM.Product.Application.Features.DiningOptions.DTOs;
using System.Collections.Generic;

namespace CRM.Product.Application.Features.DiningOptions.Queries
{
    public class GetDiningOptionListQuery : IRequest<IEnumerable<DiningOptionDto>>
    {
    }
}
