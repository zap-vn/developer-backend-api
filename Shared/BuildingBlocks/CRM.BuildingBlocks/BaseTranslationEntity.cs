using System;

namespace CRM.BuildingBlocks
{
    public abstract class BaseTranslationEntity : BaseEntity
    {
        public Guid EntityId { get; set; }
        public string LanguageCode { get; set; } = "vi-VN";
    }
}
