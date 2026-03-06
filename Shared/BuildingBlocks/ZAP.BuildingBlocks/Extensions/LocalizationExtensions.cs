using System.Linq;
using ZAP.BuildingBlocks.Interfaces;

namespace ZAP.BuildingBlocks.Extensions
{
    public static class LocalizationExtensions
    {
        public static TTranslation? GetTranslation<TTranslation>(this ILocalizable<TTranslation> entity, string? languageCode = null)
            where TTranslation : BaseTranslationEntity
        {
            if (entity.Translations == null || !entity.Translations.Any())
                return null;

            var lang = languageCode ?? System.Globalization.CultureInfo.CurrentCulture.Name;

            // 1. Exact match (e.g., vi-VN)
            var translation = entity.Translations.FirstOrDefault(t => t.LanguageCode.Equals(lang, System.StringComparison.OrdinalIgnoreCase));

            // 2. Prefix match (e.g., if lang is 'en-GB' and we only have 'en-US')
            if (translation == null)
            {
                var langPrefix = lang.Split('-')[0];
                translation = entity.Translations.FirstOrDefault(t => t.LanguageCode.StartsWith(langPrefix, System.StringComparison.OrdinalIgnoreCase));
            }

            // 3. Fallback to default (vi-VN)
            if (translation == null)
            {
                translation = entity.Translations.FirstOrDefault(t => t.LanguageCode.Equals("vi-VN", System.StringComparison.OrdinalIgnoreCase));
            }

            return translation;
        }
    }
}
