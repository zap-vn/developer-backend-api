using System;
using MongoDB.Bson.Serialization.Attributes;

namespace CRM.BuildingBlocks
{
    [BsonIgnoreExtraElements(Inherited = true)]
    public abstract class BaseEntity
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public virtual string? UserGuid { get; set; } // MerchantId / TenantId (Example: "Customer/1")
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
