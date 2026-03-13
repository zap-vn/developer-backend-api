namespace CRM.Customer.Application.Features.Customers.DTOs
{
    public class CustomerDto
    {
        public string Id { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsVerifyEmail { get; set; }
        public bool IsVerifyPhone { get; set; }
        public bool IsVerifyGoogle { get; set; }
        public bool IsVerifyApple { get; set; }
        public string MerchantUrl { get; set; } = string.Empty;
    }
}
