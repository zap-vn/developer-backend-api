using MediatR;
using CRM.HR.Application.Employees.DTOs;

namespace CRM.HR.Application.Employees.Commands.CreateEmployee
{
    public record CreateEmployeeCommand(
        string EmployeeCode,
        string FirstName,
        string LastName,
        string Email,
        string Department
    ) : IRequest<EmployeeDto>;
}
