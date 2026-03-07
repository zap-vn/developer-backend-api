using System;
using System.Collections.Generic;
using CRM.BuildingBlocks;
using CRM.BuildingBlocks.Interfaces;

namespace CRM.Organization.Domain.Entities
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
