using MediatR;
using CRM.HR.Application.Employees.DTOs;
using CRM.HR.Domain.Interfaces;
using CRM.BuildingBlocks.Models;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CRM.HR.Application.Employees.Queries
{
    public class GetEmployeeListQueryHandler : IRequestHandler<GetEmployeeListQuery, PagedResult<EmployeeDto>>
    {
        private readonly IEmployeeRepository _repository;

        public GetEmployeeListQueryHandler(IEmployeeRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResult<EmployeeDto>> Handle(GetEmployeeListQuery request, CancellationToken cancellationToken)
        {
            var filter = request.Filter ?? new FilterDTOs();
            var pagedResult = await _repository.GetPagedAsync(filter.PageIndex, filter.PageSize);
            
            var dtos = pagedResult.Items.Select(x => new EmployeeDto
            {
                Id = x.Id ?? string.Empty,
                EmployeeCode = x.EmployeeCode,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                Department = x.Department
            }).ToList();

            return new PagedResult<EmployeeDto>(dtos, pagedResult.TotalCount, pagedResult.CurrentPage, pagedResult.PageSize);
        }
    }
}
