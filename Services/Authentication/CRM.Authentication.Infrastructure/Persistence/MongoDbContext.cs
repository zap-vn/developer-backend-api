using Microsoft.Extensions.Options;
using MongoDB.Driver;
using CRM.Authentication.Domain.Entities;
using CRM.Authentication.Infrastructure.Persistence.Configurations;

namespace CRM.Authentication.Infrastructure.Persistence
{
    public class MongoDbContext
    {
        private static IMongoClient? _client;
        public readonly IMongoDatabase Database;

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

            // Configure BsonClassMap for User to match existing schema without messing up _id
            if (!MongoDB.Bson.Serialization.BsonClassMap.IsClassMapRegistered(typeof(User)))
            {
                try 
                {
                    MongoDB.Bson.Serialization.BsonClassMap.RegisterClassMap<User>(cm =>
                    {
                        cm.AutoMap();
                        
                        // Map properties to their respective BSON elements
                        cm.MapIdProperty(u => u._id);
                        cm.MapProperty(u => u.Email).SetElementName("Email");
                        
                        // Map MerchantName to 'BusinessName' if it exists in your schema
                        cm.MapProperty(u => u.MerchantName).SetElementName("MerchantName");
                        
                        // Map other names if they differ
                        cm.MapProperty(u => u.CreatedAt).SetElementName("CreateDate");
                        cm.MapProperty(u => u.Avatar).SetElementName("Url");

                        cm.SetIgnoreExtraElements(true);
                    });
                    Console.WriteLine("[MongoDB] User class map registered.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[MongoDB Warning] ClassMap error: {ex.Message}");
                }
            }

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
                
                Console.WriteLine("[MongoDB] CustomerOtps indexes ensured.");
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
    }
}
