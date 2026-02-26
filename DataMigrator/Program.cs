using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace DataMigrator;

class Program
{
    static async Task Main(string[] args)
    {
        var atlasUri = "mongodb+srv://tommy_db_user:Tommy%40123456@cluster0.dcuwhnu.mongodb.net/SinglePoint_en?retryWrites=true&w=majority&appName=Cluster0";
        var client = new MongoClient(atlasUri);
        var db = client.GetDatabase("SinglePoint_en");
        
        var collections = await (await db.ListCollectionNamesAsync()).ToListAsync();
        Console.WriteLine("--- COLLECTIONS ---");
        foreach(var c in collections) Console.WriteLine(c);

        Console.WriteLine("\n--- DATA CHECK ---");
        string[] targets = { "Product", "Products" };
        foreach (var t in targets) {
            var coll = db.GetCollection<BsonDocument>(t);
            var doc = await coll.Find(Builders<BsonDocument>.Filter.Eq("_id", "Product/4022")).FirstOrDefaultAsync();
            if (doc != null) {
                Console.WriteLine($"COLLECTION '{t}' - FOUND:");
                Console.WriteLine(doc.ToJson(new MongoDB.Bson.IO.JsonWriterSettings { Indent = true }));
            } else {
                Console.WriteLine($"COLLECTION '{t}' - NOT FOUND");
            }
        }
    }
}
