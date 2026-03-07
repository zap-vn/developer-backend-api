using MediatR;
using CRM.Customer.Domain.Entities;

namespace CRM.Customer.Application.Features.Customers.Commands
{
    public class CreateCustomerCommand : IRequest<string>
    {
        public string _id { get; set; } = string.Empty;
        public long CustomerId { get; set; }
        public long _key { get; set; }
        public string BusinessName { get; set; } = string.Empty;
        public string MerchantName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string Password { get; set; } = string.Empty;
        public string CustomerCode { get; set; } = string.Empty;
        public int Visible { get; set; } = 1;
        public string LanguageId { get; set; } = string.Empty;
        public string RegistrationSource { get; set; } = "Email";
        public string Url { get; set; } = string.Empty;

        // Legacy compat (only if strictly necessary for existing handlers, otherwise can be removed if moved to features)
        public string? Name { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
