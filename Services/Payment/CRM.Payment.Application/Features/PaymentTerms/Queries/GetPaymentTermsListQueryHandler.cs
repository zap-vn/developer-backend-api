using MediatR;
using CRM.Payment.Application.Features.PaymentTerms.DTOs;
using CRM.BuildingBlocks.Models;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using CRM.Payment.Domain.Interfaces;

namespace CRM.Payment.Application.Features.PaymentTerms.Queries
{
    public class GetPaymentTermsListQueryHandler : IRequestHandler<GetPaymentTermsListQuery, PagedResult<PaymentTermsDto>>
    {
        private readonly IPaymentTermsRepository _repository;

        public GetPaymentTermsListQueryHandler(IPaymentTermsRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResult<PaymentTermsDto>> Handle(GetPaymentTermsListQuery request, CancellationToken cancellationToken)
        {
            var list = await _repository.GetAllAsync();
            var dtos = list.Select(x => new PaymentTermsDto 
            { 
#pragma warning disable CS8602
                Id = x.Id.ToString(),
                Code = x.Code,
                Name = x.Name,
                Description = x.Description,
                Days = x.Days,
                IsActive = x.IsActive
#pragma warning restore CS8602
            }).ToList();

            return new PagedResult<PaymentTermsDto>(dtos, dtos.Count, request.Filter.Page, request.Filter.PageSize);
        }
    }
}
