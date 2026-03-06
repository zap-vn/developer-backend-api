using System.Collections.Generic;
using ZAP.BuildingBlocks;
using ZAP.BuildingBlocks.Interfaces;

namespace ZAP.Sales.Domain.Entities.Reports
{
    public class ReportTemplate : BaseEntity, ILocalizable<ReportTemplateTranslation>
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty; // Fallback
        public string Type { get; set; } = "Revenue"; // Revenue, Stock, Sales, etc.
        public string ConfigurationJson { get; set; } = string.Empty;
        
        public ICollection<ReportTemplateTranslation> Translations { get; set; } = new List<ReportTemplateTranslation>();
    }
}
