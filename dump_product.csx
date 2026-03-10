using MongoDB.Bson;
using MongoDB.Driver;
using System;

var client = new MongoClient("mongodb+srv://tommy_db_user:Tommy123456@cluster0.dcuwhnu.mongodb.net/SinglePoint_en?connectTimeoutMS=10000&serverSelectionTimeoutMS=10000&socketTimeoutMS=10000");
var db = client.GetDatabase("SinglePoint_en");
var collection = db.GetCollection<BsonDocument>("Products");
var doc = collection.Find(new BsonDocument()).FirstOrDefault();

if (doc != null)
{
    Console.WriteLine(doc.ToJson());
}
else
{
    Console.WriteLine("No documents found in Products collection.");
}
