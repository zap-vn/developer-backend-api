using MediatR;

namespace ZAP.Payment.Application.Features.PaymentTypes.Commands
{
    public class UpdatePaymentTypeCommand : IRequest<bool>
    {
        public string Id { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
