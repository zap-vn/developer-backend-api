using MediatR;
using CRM.HR.Application.Employees.DTOs;
using CRM.BuildingBlocks.Models;

namespace CRM.HR.Application.Employees.Queries
{
    public class GetEmployeeListQuery : IRequest<PagedResult<EmployeeDto>>
    {
        public FilterDTOs? Filter { get; set; }
    }
}
