using CRM.BuildingBlocks;

namespace CRM.Sales.Domain.Entities.Reports
{
    public class ReportTemplateTranslation : BaseTranslationEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
