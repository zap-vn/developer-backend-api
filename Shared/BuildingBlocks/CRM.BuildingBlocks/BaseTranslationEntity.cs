using System;

namespace CRM.BuildingBlocks
{
    public abstract class BaseTranslationEntity : BaseEntity
    {
        public string EntityId { get; set; } = string.Empty;
        public string LanguageCode { get; set; } = "vi-VN";
    }
}
