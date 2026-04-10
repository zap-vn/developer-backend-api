using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using CRM.BuildingBlocks;
using CRM.BuildingBlocks.Interfaces;

namespace CRM.Customer.Domain.Entities
{
    public class TaxSyncSetting
    {
        public bool IsAutoSyncEnabled { get; set; }
        public int SettingTypeId { get; set; }
        public string TimePicker { get; set; } = string.Empty;
    }

    [BsonIgnoreExtraElements]
    public class CustomerEntity : BaseEntity, ILocalizable<CustomerTranslation>
    {
        public long _key { get; set; }
        public string _rev { get; set; } = string.Empty;
        public string BusinessName { get; set; } = string.Empty;
        public string BussinessTypeId { get; set; } = string.Empty;
        public int Country { get; set; }
        public string CreateDate { get; set; } = string.Empty;
        public string CurrencyId { get; set; } = string.Empty;
        public string CurrencyNativeName { get; set; } = string.Empty;
        public string CurrencySymbol { get; set; } = string.Empty;
        public string CustomerCode { get; set; } = string.Empty;
        
        [BsonSerializer(typeof(CRM.BuildingBlocks.FlexibleLongSerializer))]
        [BsonElement("LanguageId")]
        public long LanguageId { get; set; }
        
        [BsonElement("Language")]
        public string Language { get; set; } = string.Empty;
        
        public int CustomerStatusId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string EmpGuid { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string InterestGrade { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PassCode { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Point { get; set; } = string.Empty;
        public string ReferenceId { get; set; } = string.Empty;
        public string StartedDate { get; set; } = string.Empty;
        public string TimeZoneDisplayName { get; set; } = string.Empty;
        public string TimeZoneId { get; set; } = string.Empty;
        public int Visible { get; set; }
        public string Websites { get; set; } = string.Empty;
        
        [BsonElement("MerchantUrl")]
        public string MerchantUrl { get; set; } = string.Empty;
        public int Currency_key { get; set; }
        public string Plural { get; set; } = string.Empty;
        public string Singular { get; set; } = string.Empty;
        public int NotificationId { get; set; }
        public string PublicKey { get; set; } = string.Empty;
        public string AdminUpdate { get; set; } = string.Empty;
        public string BatchCode { get; set; } = string.Empty;
        public string LinkVAT { get; set; } = string.Empty;
        public int TimeReport { get; set; }
        public int TimeInvoice { get; set; }
        public TaxSyncSetting TaxSyncSetting { get; set; } = new TaxSyncSetting();
        public string MerchantName { get; set; } = string.Empty;
        public string RegistrationSource { get; set; } = "Email";
        
        public bool IsVerify { get; set; } = false;
        public bool IsVerifyPhone { get; set; } = false;
        public bool IsVerifyEmail { get; set; } = false;
        public bool IsVerifyGoogle { get; set; } = false;
        public bool IsVerifyApple { get; set; } = false;
        
        // --- Postgres Specific Fields ---
        [BsonIgnore]
        public Guid id { get; set; }
        [BsonIgnore]
        public Guid? tenant_id { get; set; }
        [BsonIgnore]
        public string? legacy_id { get; set; }
        [BsonIgnore]
        public string? email { get; set; }
        [BsonIgnore]
        public string? phone_number { get; set; }
        [BsonIgnore]
        public string? full_name { get; set; }
        [BsonIgnore]
        public string? gender { get; set; }
        [BsonIgnore]
        public DateTime? birth_date { get; set; }
        [BsonIgnore]
        public int? country_id { get; set; }
        [BsonIgnore]
        public int? province_id { get; set; }
        [BsonIgnore]
        public int? district_id { get; set; }
        [BsonIgnore]
        public int? ward_id { get; set; }
        [BsonIgnore]
        public string? zipcode { get; set; }
        [BsonIgnore]
        public int? preferred_locale_id { get; set; }
        [BsonIgnore]
        public Guid? user_id { get; set; }
        [BsonIgnore]
        public int? status_id { get; set; }
        [BsonIgnore]
        public Guid? tier_id { get; set; }
        [BsonIgnore]
        public Guid? group_id { get; set; }
        [BsonIgnore]
        public decimal? current_points_balance { get; set; }
        [BsonIgnore]
        public decimal? total_spent_amount { get; set; }

        [BsonIgnore]
        public virtual LoyaltyTier? loyalty_tier { get; set; }

        [BsonIgnore]
        public virtual StatusItem? status { get; set; }
        // -------------------------------

        // Legacy fields for compilation if needed
        public string Name { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        
        public ICollection<CustomerTranslation> Translations { get; set; } = new List<CustomerTranslation>();
    }
}
