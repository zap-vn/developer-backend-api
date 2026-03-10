using MediatR;
using CRM.Payment.Application.Features.PaymentTypes.DTOs;
using CRM.BuildingBlocks.Models;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using CRM.Payment.Domain.Interfaces;

namespace CRM.Payment.Application.Features.PaymentTypes.Queries
{
    public class GetPaymentTypeListQueryHandler : IRequestHandler<GetPaymentTypeListQuery, PagedResult<PaymentTypeDto>>
    {
        private readonly IPaymentTypeRepository _repository;

        public GetPaymentTypeListQueryHandler(IPaymentTypeRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResult<PaymentTypeDto>> Handle(GetPaymentTypeListQuery request, CancellationToken cancellationToken)
        {
            var list = await _repository.GetAllAsync();
            var dtos = list.Select(x => new PaymentTypeDto 
            { 
#pragma warning disable CS8602
                Id = x.Id.ToString(),
                Code = x.Code,
                Name = x.Name,
                Description = x.Description,
                IsActive = x.IsActive
#pragma warning restore CS8602
            }).ToList();

            return new PagedResult<PaymentTypeDto>(dtos, dtos.Count, request.Filter.PageIndex, request.Filter.PageSize);
        }
    }
}
