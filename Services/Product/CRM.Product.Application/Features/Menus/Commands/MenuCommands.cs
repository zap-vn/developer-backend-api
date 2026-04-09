using System;
using MediatR;

namespace CRM.Product.Application.Features.Menus.Commands
{
    public class CreateMenuCommand : IRequest<Guid>
    {
        public Guid? tenant_id { get; set; }
        public string name { get; set; } = string.Empty;
        public string menu_type { get; set; } = "DIGITAL";
        public string? timezone_id { get; set; }
        public bool is_active { get; set; } = true;
    }

    public class UpdateMenuCommand : IRequest<bool>
    {
        public Guid id { get; set; }
        public string name { get; set; } = string.Empty;
        public string menu_type { get; set; } = string.Empty;
        public string? timezone_id { get; set; }
        public bool is_active { get; set; }
    }

    public class DeleteMenuCommand : IRequest<bool>
    {
        public Guid id { get; set; }
    }
}
