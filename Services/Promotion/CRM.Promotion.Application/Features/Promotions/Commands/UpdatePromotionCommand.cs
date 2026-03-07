using MediatR;
using System;

namespace CRM.Promotion.Application.Features.Promotions.Commands
{
    public class UpdatePromotionCommand : IRequest<bool>
    {
        public string Id { get; set; } = string.Empty; // Injected from route map
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal DiscountValue { get; set; }
        public string DiscountType { get; set; } = string.Empty;
    }
}
