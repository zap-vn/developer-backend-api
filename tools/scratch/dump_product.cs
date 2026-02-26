using MongoDB.Driver;
using MongoDB.Bson;
using System;
using System.Threading.Tasks;

public class RawDump
{
    public static async Task Main()
    {
        var client = new MongoClient("mongodb+srv://tommy_db_user:Tommy%40123456@cluster0.dcuwhnu.mongodb.net/SinglePoint_en?retryWrites=true&w=majority&appName=Cluster0");
        var db = client.GetDatabase("SinglePoint_en");
        var collection = db.GetCollection<BsonDocument>("Products");
        
        var doc = await collection.Find(new BsonDocument("_id", "Product/4022")).FirstOrDefaultAsync();
        if (doc != null) {
            Console.WriteLine(doc.ToJson(new MongoDB.Bson.IO.JsonWriterSettings { Indent = true }));
        } else {
            Console.WriteLine("Not found");
        }
    }
}
