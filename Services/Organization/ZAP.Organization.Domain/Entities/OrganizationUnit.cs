using System;
using System.Collections.Generic;
using ZAP.BuildingBlocks;
using ZAP.BuildingBlocks.Interfaces;

namespace ZAP.Organization.Domain.Entities
{
    public class OrganizationUnit : BaseEntity, ILocalizable<OrganizationUnitTranslation>
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid? ParentId { get; set; }
        public string Type { get; set; } = "Department";
        
        public ICollection<OrganizationUnitTranslation> Translations { get; set; } = new List<OrganizationUnitTranslation>();
    }
}
