using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using CRM.BuildingBlocks;
using CRM.BuildingBlocks.Interfaces;

namespace CRM.HR.Domain.Entities
{
    public class Employee : BaseEntity, ILocalizable<EmployeeTranslation>
    {
        [BsonElement("EmpGuid")]
        public override string? UserGuid { get; set; }

        public string EmployeeCode { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty; // Fallback
        public string LastName { get; set; } = string.Empty; // Fallback
        public string Email { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public Guid? UserId { get; set; } 
        
        public ICollection<EmployeeTranslation> Translations { get; set; } = new List<EmployeeTranslation>();
    }
}
