using ZAP.BuildingBlocks;

namespace ZAP.Customer.Domain.Entities
{
    public class CustomerGroupTranslation : BaseTranslationEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
