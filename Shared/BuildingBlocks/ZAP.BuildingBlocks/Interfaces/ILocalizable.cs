using System.Collections.Generic;

namespace ZAP.BuildingBlocks.Interfaces
{
    public interface ILocalizable<TTranslation> where TTranslation : BaseTranslationEntity
    {
        ICollection<TTranslation> Translations { get; set; }
    }
}
