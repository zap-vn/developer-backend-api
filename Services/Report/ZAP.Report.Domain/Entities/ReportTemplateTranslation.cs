using ZAP.BuildingBlocks;

namespace ZAP.Report.Domain.Entities
{
    public class ReportTemplateTranslation : BaseTranslationEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
