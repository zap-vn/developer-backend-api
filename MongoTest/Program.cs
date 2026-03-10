using System;
using System.IO;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq;

var client = new MongoClient("mongodb+srv://tommy_db_user:Tommy123456@cluster0.dcuwhnu.mongodb.net/SinglePoint_en?connectTimeoutMS=10000&serverSelectionTimeoutMS=10000&socketTimeoutMS=10000");
var db = client.GetDatabase("SinglePoint_en");
// Try both "Product" and "Products" as the collection name
var collNames = new[] { "Product", "Products" };

foreach (var name in collNames)
{
    var coll = db.GetCollection<BsonDocument>(name);
    var res = coll.Find(new BsonDocument()).Limit(1).FirstOrDefault();
    if (res != null)
    {
        Console.WriteLine($"COLLECTION: {name}");
        Console.WriteLine(res.ToJson(new MongoDB.Bson.IO.JsonWriterSettings { Indent = true }));
        File.WriteAllText($"d:/PROJECTS/2026/3_2/src/dump_{name}.txt", res.ToJson(new MongoDB.Bson.IO.JsonWriterSettings { Indent = true }));
    }
}
