using MongoDB.Bson;
using MongoDB.Driver;
using System;

Console.WriteLine("--> Inspecting Product/4022...");
var connectionString = "mongodb://172.16.10.153:27017/?retryWrites=false&loadBalanced=false&connectTimeoutMS=10000";
var client = new MongoClient(connectionString);
var database = client.GetDatabase("SinglePoint_en");
var collection = database.GetCollection<BsonDocument>("Product");
var filter = Builders<BsonDocument>.Filter.Eq("_id", "Product/4022");
var doc = collection.Find(filter).FirstOrDefault();

if (doc != null)
{
    Console.WriteLine("✅ Document found:");
    Console.WriteLine(doc.ToJson(new MongoDB.Bson.IO.JsonWriterSettings { Indent = true }));
}
else
{
    Console.WriteLine("❌ Document Product/4022 NOT found in SinglePoint_en.Product");
}
