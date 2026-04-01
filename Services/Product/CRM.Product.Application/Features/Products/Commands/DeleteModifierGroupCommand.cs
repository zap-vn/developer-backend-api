using System;
using MediatR;

namespace CRM.Product.Application.Features.Products.Commands
{
    public class DeleteModifierGroupCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
    }
}
