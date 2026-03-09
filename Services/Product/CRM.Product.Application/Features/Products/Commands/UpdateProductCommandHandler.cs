using MediatR;
using System.Threading;
using System.Threading.Tasks;
using CRM.Product.Domain.Interfaces;
using System;

namespace CRM.Product.Application.Features.Products.Commands
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, bool>
    {
        private readonly IProductRepository _repository;

        public UpdateProductCommandHandler(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Id)) return false;
            var entity = await _repository.GetByIdAsync(request.Id);
            if (entity == null) return false;

            entity.Name = request.Name;
            entity.Description = request.Description;
            entity.Price = request.Price;
            entity.Stock = request.Stock;
            entity.Category = request.Category;
            entity.ImageUrl = request.ImageUrl;

            await _repository.UpdateAsync(entity);
            return true;
        }
    }
}
