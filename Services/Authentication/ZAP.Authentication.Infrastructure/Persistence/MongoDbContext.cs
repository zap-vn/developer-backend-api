using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ZAP.Authentication.Domain.Entities;
using ZAP.Authentication.Infrastructure.Persistence.Configurations;

namespace ZAP.Authentication.Infrastructure.Persistence
{
    public class MongoDbContext
    {
        private static IMongoClient? _client;
        private readonly IMongoDatabase _database;

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
                    _client = new MongoClient(clientSettings);
                }
                catch (Exception)
                {
                    throw;
                }
            }
            _database = _client.GetDatabase(mongoSettings.DatabaseName);

            // Configure BsonClassMap for User to match existing schema without messing up _id
            if (!MongoDB.Bson.Serialization.BsonClassMap.IsClassMapRegistered(typeof(User)))
            {
                try 
                {
                    MongoDB.Bson.Serialization.BsonClassMap.RegisterClassMap<User>(cm =>
                    {
                        cm.AutoMap();
                        
                        // Fix dual-mapping of 'Email' BSON element
                        cm.UnmapProperty(u => u.Email);
                        
                        // Map _id property to the _id element in MongoDB
                        cm.MapIdProperty(u => u._id);
                        
                        // Map Username to the legacy 'Email' element in database
                        cm.MapProperty(u => u.Username).SetElementName("Email");

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
                    Console.WriteLine($"[MongoDB Warning] ClassMap error (likely already registered): {ex.Message}");
                }
            }
        }

        public IMongoCollection<User> Users => _database.GetCollection<User>("Customer");
    }
}
