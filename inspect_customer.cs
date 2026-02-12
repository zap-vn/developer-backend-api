using MongoDB.Driver;
using MongoDB.Bson;
using System;

var client = new MongoClient("mongodb://172.16.10.153:27017/");
using var cursor = await client.ListDatabaseNamesAsync();
await cursor.ForEachAsync(dbName => Console.WriteLine(dbName));
