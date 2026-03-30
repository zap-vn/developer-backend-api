using MediatR;
using CRM.HR.Application.Employees.DTOs;
using CRM.HR.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace CRM.HR.Application.Employees.Queries
{
    public class GetEmployeeByIdQueryHandler : IRequestHandler<GetEmployeeByIdQuery, EmployeeDto?>
    {
        private readonly IEmployeeRepository _repository;

        public GetEmployeeByIdQueryHandler(IEmployeeRepository repository)
        {
            _repository = repository;
        }

        public async Task<EmployeeDto?> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Id)) return null;
            var entity = await _repository.GetByIdAsync(request.Id);
            if (entity == null) return null;

            return new EmployeeDto
            {
                Id = entity.Id ?? string.Empty,
                EmployeeCode = entity.EmployeeCode,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                Email = entity.Email,
                Department = entity.Department
            };
        }
    }
}
