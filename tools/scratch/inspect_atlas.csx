// Script to inspect MongoDB Atlas collections and their structure
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.IO;

var connectionString = "mongodb+srv://tommy_db_user:Tommy%40123456@cluster0.dcuwhnu.mongodb.net/SinglePoint_en?appName=Cluster0&connectTimeoutMS=10000&serverSelectionTimeoutMS=10000";

var client = new MongoClient(connectionString);

// List all databases
Console.WriteLine("=== DATABASES ===");
var dbNames = client.ListDatabaseNames().ToList();
foreach (var db in dbNames)
{
    Console.WriteLine($"  DB: {db}");
}

Console.WriteLine("\n=== COLLECTIONS IN SinglePoint_en ===");
var database = client.GetDatabase("SinglePoint_en");
var collections = database.ListCollectionNames().ToList();
collections.Sort();

foreach (var collName in collections)
{
    var collection = database.GetCollection<BsonDocument>(collName);
    var count = collection.CountDocuments(new BsonDocument());
    Console.WriteLine($"\n--- {collName} (Documents: {count}) ---");
    
    // Get first document to show structure
    var sample = collection.Find(new BsonDocument()).Limit(1).FirstOrDefault();
    if (sample != null)
    {
        var fields = new List<string>();
        foreach (var element in sample.Elements)
        {
            var typeName = element.Value.BsonType.ToString();
            var valuePreview = "";
            if (element.Value.BsonType == BsonType.String)
            {
                var str = element.Value.AsString;
                valuePreview = str.Length > 50 ? $" = \"{str.Substring(0, 50)}...\"" : $" = \"{str}\"";
            }
            else if (element.Value.BsonType == BsonType.Int32)
                valuePreview = $" = {element.Value.AsInt32}";
            else if (element.Value.BsonType == BsonType.Int64)
                valuePreview = $" = {element.Value.AsInt64}";
            else if (element.Value.BsonType == BsonType.Boolean)
                valuePreview = $" = {element.Value.AsBoolean}";
            else if (element.Value.BsonType == BsonType.Array)
                valuePreview = $" [{element.Value.AsBsonArray.Count} items]";
            else if (element.Value.BsonType == BsonType.Document)
                valuePreview = $" {{{element.Value.AsBsonDocument.ElementCount} fields}}";
                
            fields.Add($"    {element.Name}: {typeName}{valuePreview}");
        }
        Console.WriteLine(string.Join("\n", fields));
    }
}

Console.WriteLine("\n=== DONE ===");
