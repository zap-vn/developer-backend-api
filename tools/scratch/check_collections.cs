using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

var atlasUri = "mongodb+srv://tommy_db_user:Tommy123456@cluster0.dcuwhnu.mongodb.net/SinglePoint_en";
var client = new MongoClient(atlasUri);
var database = client.GetDatabase("SinglePoint_en");

var collections = await (await database.ListCollectionNamesAsync()).ToListAsync();
Console.WriteLine("Collections in SinglePoint_en:");
foreach (var name in collections)
{
    Console.WriteLine($"- {name}");
}

if (collections.Contains("CRMResourceMaps"))
{
    var col = database.GetCollection<BsonDocument>("CRMResourceMaps");
    var doc = await col.Find(new BsonDocument()).Limit(1).FirstOrDefaultAsync();
    if (doc != null)
    {
        Console.WriteLine("\nSample CRMResourceMaps document:");
        Console.WriteLine(doc.ToJson(new MongoDB.Bson.IO.JsonWriterSettings { Indent = true }));
    }
}
else 
{
    Console.WriteLine("\nCRMResourceMaps collection NOT found in SinglePoint_en.");
}
