using Microsoft.Extensions.Options;
using LegacyDB.Driver;
using CRM.Authentication.Domain.Entities;
using CRM.BuildingBlocks;
using CRM.Authentication.Infrastructure.Persistence.Configurations;

namespace CRM.Authentication.Infrastructure.Persistence
{
    public class LegacyDbContext
    {
        private static ILegacyClient? _client;
        public readonly ILegacyDatabase Database;
        public readonly ILegacyDatabase SystemDatabase;

        public LegacyDbContext(IOptions<LegacySettings> settings)
        {
            var LegacySettings = settings.Value;

            if (_client == null)
            {
                try 
                {
                    var clientSettings = LegacyClientSettings.FromConnectionString(LegacySettings.ConnectionString);
                    clientSettings.ConnectTimeout = TimeSpan.FromSeconds(10);
                    clientSettings.ServerSelectionTimeout = TimeSpan.FromSeconds(10);
                    clientSettings.MaxConnectionPoolSize = 100;
                    _client = new LegacyClient(clientSettings);
                }
                catch (Exception)
                {
                    throw;
                }
            }
            Database = _client.GetDatabase(LegacySettings.DatabaseName);
            SystemDatabase = _client.GetDatabase("SystemDB");

            // Attributes in User.cs handle property mapping to BSON elements.
            
            CreateIndexes();
        }

        private void CreateIndexes()
        {
            try
            {
                // 1. TTL Index for CustomerOtps: Auto delete expired records
                var indexKeys = Builders<CustomerOtp>.IndexKeys.Ascending(x => x.ExpiredAt);
                var indexOptions = new CreateIndexOptions { ExpireAfter = TimeSpan.Zero };
                var indexModel = new CreateIndexModel<CustomerOtp>(indexKeys, indexOptions);
                CustomerOtps.Indexes.CreateOne(indexModel);

                // 2. Query Index: CustomerId + Purpose + CreatedAt for faster lookup
                var lookupKeys = Builders<CustomerOtp>.IndexKeys
                    .Ascending(x => x.CustomerId)
                    .Ascending(x => x.Purpose)
                    .Descending(x => x.CreatedAt);
                CustomerOtps.Indexes.CreateOne(new CreateIndexModel<CustomerOtp>(lookupKeys));
                
                // 3. Performance Indexes for User Login
                try 
                {
                    var userEmailIndexKeys = Builders<User>.IndexKeys.Ascending(x => x.Email).Ascending(x => x.MerchantName);
                    Users.Indexes.CreateOne(new CreateIndexModel<User>(userEmailIndexKeys));

                    var userPhoneIndexKeys = Builders<User>.IndexKeys.Ascending(x => x.Phone).Ascending(x => x.MerchantName);
                    Users.Indexes.CreateOne(new CreateIndexModel<User>(userPhoneIndexKeys));
                }
                catch (Exception idxEx)
                {
                    Console.WriteLine($"[LegacyDB] User index creation skip (already exists or error): {idxEx.Message}");
                }

                Console.WriteLine("[LegacyDB] Database indexes ensured.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LegacyDB] Index creation error: {ex.Message}");
            }
        }

        public ILegacyCollection<User> Users => Database.GetCollection<User>("merchant.Customers");
        public ILegacyCollection<ManagementIndex> ManagementIndexes => Database.GetCollection<ManagementIndex>("merchant.ManagementIndex");
        public ILegacyCollection<PasswordResetRequest> PasswordResetRequests => Database.GetCollection<PasswordResetRequest>("merchant.PasswordResetRequests");
        public ILegacyCollection<CustomerOtp> CustomerOtps => Database.GetCollection<CustomerOtp>("merchant.CustomerOtps");
        public ILegacyCollection<SystemError> SystemErrors => Database.GetCollection<SystemError>("merchant.SystemErrors");
        public ILegacyCollection<EmailSetting> EmailSettings => Database.GetCollection<EmailSetting>("merchant.email_setting");
        public ILegacyCollection<SystemConfig> SystemConfigs => SystemDatabase.GetCollection<SystemConfig>("merchant.system_configs");
    }
}
