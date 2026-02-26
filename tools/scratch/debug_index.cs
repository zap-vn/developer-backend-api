using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

public class DebugIndex
{
    public static async Task Run()
    {
        var client = new MongoClient("mongodb+srv://tommy_db_user:Tommy123456@cluster0.dcuwhnu.mongodb.net/SinglePoint_en");
        var db = client.GetDatabase("SinglePoint_en");
        var coll = db.GetCollection<BsonDocument>("ManagementIndex");
        
        var filter = Builders<BsonDocument>.Filter.Eq("_id", "Customer_id");
        var doc = await coll.Find(filter).FirstOrDefaultAsync();
        
        if (doc == null)
        {
            Console.WriteLine("ManagementIndex document 'Customer_id' not found!");
        }
        else
        {
            Console.WriteLine($"Found: {doc.ToJson()}");
        }
    }
}
