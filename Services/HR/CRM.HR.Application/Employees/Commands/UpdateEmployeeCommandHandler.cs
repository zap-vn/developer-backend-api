using MediatR;
using CRM.HR.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace CRM.HR.Application.Employees.Commands
{
    public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, bool>
    {
        private readonly IEmployeeRepository _repository;

        public UpdateEmployeeCommandHandler(IEmployeeRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.Id);
            if (entity == null) return false;

            entity.EmployeeCode = request.EmployeeCode;
            entity.FirstName = request.FirstName;
            entity.LastName = request.LastName;
            entity.Email = request.Email;
            entity.Department = request.Department;

            return await _repository.UpdateAsync(entity);
        }
    }
}
