using CRM.BuildingBlocks;

namespace CRM.Organization.Domain.Entities
{
    public class OrganizationUnitTranslation : BaseTranslationEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
