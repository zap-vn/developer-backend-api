using System;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Threading.Tasks;

var client = new MongoClient("mongodb+srv://tommy_db_user:Tommy123456@cluster0.dcuwhnu.mongodb.net/SinglePoint_en?connectTimeoutMS=5000");
var db = client.GetDatabase("SinglePoint_en");
var collection = db.GetCollection<BsonDocument>("SystemLanguages");

Console.WriteLine("Fetching SystemLanguages...");
var languages = await collection.Find(new BsonDocument()).ToListAsync();
Console.WriteLine($"Found {languages.Count} languages.");
foreach (var lang in languages)
{
    Console.WriteLine(lang.ToJson());
}
