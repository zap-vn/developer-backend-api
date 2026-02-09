using MongoDB.Bson;
using MongoDB.Driver;
using System;

Console.WriteLine("Starting MongoDB debug...");
var connectionString = "mongodb://172.16.10.153:27017/?retryWrites=false&loadBalanced=false&connectTimeoutMS=10000";
var client = new MongoClient(connectionString);
var database = client.GetDatabase("SinglePoint_en");
var collection = database.GetCollection<BsonDocument>("Customer");
var doc = collection.Find(new BsonDocument()).FirstOrDefault();

if (doc != null)
{
    Console.WriteLine(doc.ToJson(new MongoDB.Bson.IO.JsonWriterSettings { Indent = true }));
}
else
{
    Console.WriteLine("No documents found.");
}
