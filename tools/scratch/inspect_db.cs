using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

public class InspectDB
{
    public static async Task Main()
    {
        try {
            var client = new MongoClient("mongodb://172.16.10.153:27017");
            var db = client.GetDatabase("SinglePoint_en");
            
            // Check Index
            var indexColl = db.GetCollection<BsonDocument>("ManagementIndex");
            var index = await indexColl.Find(Builders<BsonDocument>.Filter.Eq("_id", "Customer_id")).FirstOrDefaultAsync();
            Console.WriteLine($"ManagementIndex: {index?.ToJson()}");
            
            // Check Last Customers
            var custColl = db.GetCollection<BsonDocument>("Customer");
            var lastCusts = await custColl.Find(new BsonDocument()).SortByDescending(d => d["_id"]).Limit(3).ToListAsync();
            Console.WriteLine("Last 3 Customers in DB:");
            foreach(var c in lastCusts) Console.WriteLine(c.ToJson());
            
        } catch (Exception ex) {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
