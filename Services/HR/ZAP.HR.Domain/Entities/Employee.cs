using System;
using ZAP.BuildingBlocks;

namespace ZAP.HR.Domain.Entities
{
    public class Employee : BaseEntity
    {
        public string EmployeeCode { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public Guid? UserId { get; set; } // Link to Authentication User
    }
}
