using CRM.BuildingBlocks;

namespace CRM.Customer.Domain.Entities
{
    public class CustomerTranslation : BaseTranslationEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }
}
