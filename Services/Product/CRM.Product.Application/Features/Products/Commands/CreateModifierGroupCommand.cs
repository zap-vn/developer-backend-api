using System;
using MediatR;

namespace CRM.Product.Application.Features.Products.Commands
{
    public class CreateModifierGroupCommand : IRequest<Guid>
    {
        public Guid TenantId { get; set; }
        public string? LegacyId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int MinSelection { get; set; }
        public int MaxSelection { get; set; }
        public bool IsRequired { get; set; }
        public int SortOrder { get; set; }
    }
}
