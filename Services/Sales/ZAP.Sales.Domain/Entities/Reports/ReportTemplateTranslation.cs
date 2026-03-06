using ZAP.BuildingBlocks;

namespace ZAP.Sales.Domain.Entities.Reports
{
    public class ReportTemplateTranslation : BaseTranslationEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
