using MediatR;
using CRM.HR.Application.Employees.DTOs;
using CRM.HR.Domain.Entities;
using CRM.HR.Domain.Interfaces;
using CRM.BuildingBlocks.Interfaces;

namespace CRM.HR.Application.Employees.Commands.CreateEmployee
{
    public class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, EmployeeDto>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ICurrentUserService _currentUserService;

        public CreateEmployeeCommandHandler(IEmployeeRepository employeeRepository, ICurrentUserService currentUserService)
        {
            _employeeRepository = employeeRepository;
            _currentUserService = currentUserService;
        }

        public async Task<EmployeeDto> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
        {
            var employee = new Employee
            {
                EmployeeCode = request.EmployeeCode,
                Email = request.Email,
                UserId = null // Can be linked if needed
            };

            await _employeeRepository.CreateAsync(employee);

            var translation = new EmployeeTranslation
            {
                EntityId = employee.Id,
                LanguageCode = _currentUserService.LanguageCode,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Department = request.Department
            };

            await _employeeRepository.UpsertTranslationAsync(translation);

            return new EmployeeDto
            {
                Id = employee.Id,
                EmployeeCode = employee.EmployeeCode,
                FirstName = translation.FirstName,
                LastName = translation.LastName,
                Email = employee.Email,
                Department = translation.Department
            };
        }
    }
}
