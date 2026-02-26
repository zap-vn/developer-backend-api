using MongoDB.Driver;
using MongoDB.Bson;
using System;
using System.Threading.Tasks;

public class TestAtlas
{
    public static async Task Main()
    {
        var atlasUri = "mongodb+srv://tommy_db_user:Tommy%40123456@cluster0.dcuwhnu.mongodb.net/SinglePoint_en?retryWrites=true&w=majority&appName=Cluster0";
        try {
            var client = new MongoClient(atlasUri);
            var database = client.GetDatabase("SinglePoint_en");
            var collection = database.GetCollection<BsonDocument>("Products");
            
            var doc = await collection.Find(new BsonDocument("_id", "Product/4022")).FirstOrDefaultAsync();
            if (doc != null) {
                Console.WriteLine("PRODUCT_4022_RAW_START");
                Console.WriteLine(doc.ToJson(new MongoDB.Bson.IO.JsonWriterSettings { Indent = true }));
                Console.WriteLine("PRODUCT_4022_RAW_END");
            } else {
                Console.WriteLine("Product/4022 NOT FOUND");
            }
        } catch (Exception ex) {
            Console.WriteLine($"FAILED: {ex.Message}");
        }
    }
}
