using System.Collections.Generic;

namespace CRM.BuildingBlocks.Interfaces
{
    public interface ILocalizable<TTranslation> where TTranslation : BaseTranslationEntity
    {
        ICollection<TTranslation> Translations { get; set; }
    }
}
