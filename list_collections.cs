using MongoDB.Driver;
using MongoDB.Bson;
using System;
using System.IO;
using System.Threading.Tasks;

var client = new MongoClient("mongodb://172.16.10.153:27017/");
var db = client.GetDatabase("SinglePoint_en");
var collections = await (await db.ListCollectionNamesAsync()).ToListAsync();

Console.WriteLine("Collections found:");
foreach (var name in collections)
{
    Console.WriteLine(name);
}
File.WriteAllLines("collections.txt", collections);
