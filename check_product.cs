using MongoDB.Driver;
using MongoDB.Bson;
using System;
using System.Threading.Tasks;

var client = new MongoClient("mongodb://172.16.10.153:27017/?retryWrites=false&loadBalanced=false&connectTimeoutMS=10000");
var db = client.GetDatabase("SinglePoint_en");
var collection = db.GetCollection<BsonDocument>("Product");

// Tìm document có _id = "Product/4021"
var filter = Builders<BsonDocument>.Filter.Eq("_id", "Product/4021");
var doc = await collection.Find(filter).FirstOrDefaultAsync();

if (doc != null)
{
    Console.WriteLine("✅ Document found!");
    Console.WriteLine($"_id: {doc["_id"]}");
    Console.WriteLine($"Full document: {doc.ToJson()}");
}
else
{
    Console.WriteLine("❌ Document NOT found with _id = 'Product/4021'");
    
    // Thử tìm bất kỳ document nào
    var anyDoc = await collection.Find(new BsonDocument()).Limit(1).FirstOrDefaultAsync();
    if (anyDoc != null)
    {
        Console.WriteLine($"\nSample document _id format: {anyDoc["_id"]} (Type: {anyDoc["_id"].BsonType})");
    }
}
