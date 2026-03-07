using System;
using System.Collections.Generic;
using CRM.BuildingBlocks;
using CRM.BuildingBlocks.Interfaces;

namespace CRM.Sales.Domain.Entities.Organizations
{
    public class OrganizationUnit : BaseEntity, ILocalizable<OrganizationUnitTranslation>
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty; // Fallback
        public string Description { get; set; } = string.Empty; // Fallback
        public Guid? ParentId { get; set; }
        public string Type { get; set; } = "Department"; // Company, Branch, Department, Team
        
        public ICollection<OrganizationUnitTranslation> Translations { get; set; } = new List<OrganizationUnitTranslation>();
    }
}
