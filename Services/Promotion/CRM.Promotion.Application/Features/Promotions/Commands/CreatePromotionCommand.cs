using MediatR;
using System;

namespace CRM.Promotion.Application.Features.Promotions.Commands
{
    public class CreatePromotionCommand : IRequest<string>
    {
        public string Code { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal DiscountValue { get; set; }
        public string DiscountType { get; set; } = string.Empty;
    }
}
