using MediatR;
using CRM.HR.Application.Employees.DTOs;

namespace CRM.HR.Application.Employees.Queries
{
    public class GetEmployeeByIdQuery : IRequest<EmployeeDto?>
    {
        public string Id { get; set; }

        public GetEmployeeByIdQuery(string id)
        {
            Id = id;
        }
    }
}
