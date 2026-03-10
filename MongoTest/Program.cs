using System;
using System.IO;
using MongoDB.Bson;
using MongoDB.Driver;

var client = new MongoClient("mongodb+srv://tommy_db_user:Tommy123456@cluster0.dcuwhnu.mongodb.net/SinglePoint_en?connectTimeoutMS=10000&serverSelectionTimeoutMS=10000&socketTimeoutMS=10000");
var db = client.GetDatabase("SinglePoint_en");
var coll = db.GetCollection<BsonDocument>("Products");
var res = coll.Find(new BsonDocument()).Limit(1).FirstOrDefault();

if(res != null) {
  File.WriteAllText("d:/PROJECTS/2026/3_2/src/dump_product_result.txt", res.ToJson());
} else {
  File.WriteAllText("d:/PROJECTS/2026/3_2/src/dump_product_result.txt", "NULL");
}
