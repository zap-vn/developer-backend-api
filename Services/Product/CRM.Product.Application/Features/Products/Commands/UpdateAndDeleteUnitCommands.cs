using System;
using MediatR;

namespace CRM.Product.Application.Features.Products.Commands
{
    public class UpdateUnitCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? UomType { get; set; }
        public bool? IsActive { get; set; }
    }

    public class DeleteUnitCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
}
