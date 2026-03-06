using MediatR;
using ZAP.Customer.Domain.Entities;

namespace ZAP.Customer.Application.Features.Customers.Commands
{
    public class CreateCustomerCommand : IRequest<string>
    {
        public string BusinessName { get; set; } = string.Empty;
        public string MerchantName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string CustomerCode { get; set; } = string.Empty;
        public int Visible { get; set; } = 1;
        public string LanguageId { get; set; } = string.Empty;
        public string RegistrationSource { get; set; } = "Email";

        // Legacy compat
        public string Name { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
