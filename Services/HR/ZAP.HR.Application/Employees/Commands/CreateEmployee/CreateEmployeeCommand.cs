using MediatR;
using ZAP.HR.Application.Employees.DTOs;

namespace ZAP.HR.Application.Employees.Commands.CreateEmployee
{
    public record CreateEmployeeCommand(
        string EmployeeCode,
        string FirstName,
        string LastName,
        string Email,
        string Department
    ) : IRequest<EmployeeDto>;
}
