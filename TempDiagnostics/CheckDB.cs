using MongoDB.Driver;
using MongoDB.Bson;
using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        string connectionString = "mongodb+srv://tommy_db_user:Tommy123456@cluster0.dcuwhnu.mongodb.net/?appName=Cluster0";
        string databaseName = "SinglePoint_en";
        
        try
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            var indexCol = database.GetCollection<BsonDocument>("ManagementIndex");
            var customerCol = database.GetCollection<BsonDocument>("Customer");
            
            Console.WriteLine("--- ManagementIndex ---");
            var indexes = await indexCol.Find(new BsonDocument()).ToListAsync();
            foreach (var idx in indexes) Console.WriteLine(idx.ToJson());
            
            Console.WriteLine("\n--- Last 5 Customers ---");
            var customers = await customerCol.Find(new BsonDocument()).Sort(Builders<BsonDocument>.Sort.Descending("_id")).Limit(5).ToListAsync();
            foreach (var c in customers) Console.WriteLine(c.ToJson());
            
            if (indexes.Count == 0 && customers.Count == 0) Console.WriteLine("Database is empty or collection names are wrong.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
}
