using Microsoft.Extensions.Options;
using MongoDB.Driver;
using CRM.Authentication.Domain.Entities;
using CRM.BuildingBlocks;
using CRM.Authentication.Infrastructure.Persistence.Configurations;

namespace CRM.Authentication.Infrastructure.Persistence
{
    public class MongoDbContext
    {
        private static IMongoClient? _client;
        public readonly IMongoDatabase Database;
        public readonly IMongoDatabase SystemDatabase;

        public MongoDbContext(IOptions<MongoSettings> settings)
        {
            var mongoSettings = settings.Value;

            if (_client == null)
            {
                try 
                {
                    var clientSettings = MongoClientSettings.FromConnectionString(mongoSettings.ConnectionString);
                    clientSettings.ConnectTimeout = TimeSpan.FromSeconds(10);
                    clientSettings.ServerSelectionTimeout = TimeSpan.FromSeconds(10);
                    clientSettings.MaxConnectionPoolSize = 100;
                    _client = new MongoClient(clientSettings);
                }
                catch (Exception)
                {
                    throw;
                }
            }
            Database = _client.GetDatabase(mongoSettings.DatabaseName);
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
                    Console.WriteLine($"[MongoDB] User index creation skip (already exists or error): {idxEx.Message}");
                }

                Console.WriteLine("[MongoDB] Database indexes ensured.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MongoDB] Index creation error: {ex.Message}");
            }
        }

        public IMongoCollection<User> Users => Database.GetCollection<User>("Customer");
        public IMongoCollection<ManagementIndex> ManagementIndexes => Database.GetCollection<ManagementIndex>("ManagementIndex");
        public IMongoCollection<PasswordResetRequest> PasswordResetRequests => Database.GetCollection<PasswordResetRequest>("PasswordResetRequests");
        public IMongoCollection<CustomerOtp> CustomerOtps => Database.GetCollection<CustomerOtp>("CustomerOtps");
        public IMongoCollection<SystemError> SystemErrors => Database.GetCollection<SystemError>("SystemErrors");
        public IMongoCollection<EmailSetting> EmailSettings => Database.GetCollection<EmailSetting>("email_setting");
        public IMongoCollection<SystemConfig> SystemConfigs => SystemDatabase.GetCollection<SystemConfig>("system_configs");
    }
}
