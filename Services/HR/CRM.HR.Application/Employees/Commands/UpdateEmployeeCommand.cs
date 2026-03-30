using MediatR;

namespace CRM.HR.Application.Employees.Commands
{
    public class UpdateEmployeeCommand : IRequest<bool>
    {
        public string Id { get; set; } = string.Empty;
        public string EmployeeCode { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
    }
}
