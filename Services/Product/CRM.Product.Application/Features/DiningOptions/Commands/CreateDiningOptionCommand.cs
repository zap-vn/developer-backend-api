using MediatR;

namespace CRM.Product.Application.Features.DiningOptions.Commands
{
    public class CreateDiningOptionCommand : IRequest<int>
    {
        public string Code { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public int SortOrder { get; set; } = 0;
    }
}
