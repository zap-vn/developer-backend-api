using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

public class CheckCustomer
{
    public static async Task Main()
    {
        try {
            var client = new MongoClient("mongodb://172.16.10.153:27017");
            var db = client.GetDatabase("SinglePoint_en");
            var coll = db.GetCollection<BsonDocument>("Customer");
            var doc = await coll.Find(Builders<BsonDocument>.Filter.Eq("_id", "Customer/106")).FirstOrDefaultAsync();
            if (doc != null) {
                Console.WriteLine("FOUND CUSTOMER 106:");
                Console.WriteLine(doc.ToJson(new MongoDB.Bson.IO.JsonWriterSettings { Indent = true }));
            } else {
                Console.WriteLine("Customer/106 NOT FOUND in SinglePoint_en.Customer");
                var last = await coll.Find(new BsonDocument()).SortByDescending(d => d["_id"]).Limit(5).ToListAsync();
                Console.WriteLine("Last 5 customers:");
                foreach(var d in last) Console.WriteLine($" - {d["_id"]}");
            }
        } catch (Exception ex) {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
