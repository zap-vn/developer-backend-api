// Script to analyze MongoDB data consistency
#r "nuget: MongoDB.Driver, 2.23.1"
using MongoDB.Driver;
using MongoDB.Bson;
using System.Text.RegularExpressions;

var connectionString = "mongodb+srv://tommy_db_user:Tommy%40123456@cluster0.dcuwhnu.mongodb.net/SinglePoint_en?appName=Cluster0&connectTimeoutMS=10000&serverSelectionTimeoutMS=10000";
var client = new MongoClient(connectionString);
var database = client.GetDatabase("SinglePoint_en");
var collections = database.ListCollectionNames().ToList();
collections.Sort();

Console.WriteLine($"Found {collections.Count} collections. Starting analysis...\n");

var idTypesOverall = new Dictionary<string, long>();
var visibleTypesOverall = new Dictionary<string, long>();
var createDateTypesOverall = new Dictionary<string, long>();
var createDateFormats = new Dictionary<string, long>();

foreach (var collName in collections)
{
    var collection = database.GetCollection<BsonDocument>(collName);
    var count = collection.CountDocuments(new BsonDocument());
    if (count == 0) continue;

    Console.WriteLine($"Analyzing {collName} ({count} documents)...");

    // 1. Analyze _id types
    var idPipeline = new[] {
        new BsonDocument("$group", new BsonDocument {
            { "_id", new BsonDocument("$type", "$_id") },
            { "count", new BsonDocument("$sum", 1) }
        })
    };
    var idResults = collection.Aggregate<BsonDocument>(idPipeline).ToList();
    foreach (var res in idResults) {
        var type = res["_id"].ToString();
        var c = res["count"].AsInt32;
        if (!idTypesOverall.ContainsKey(type)) idTypesOverall[type] = 0;
        idTypesOverall[type] += c;
    }

    // 2. Analyze Visible types
    var visiblePipeline = new[] {
        new BsonDocument("$group", new BsonDocument {
            { "_id", new BsonDocument("$type", "$Visible") },
            { "count", new BsonDocument("$sum", 1) }
        })
    };
    var visibleResults = collection.Aggregate<BsonDocument>(visiblePipeline).ToList();
    foreach (var res in visibleResults) {
        var type = res["_id"].ToString();
        var c = res["count"].AsInt32;
        if (!visibleTypesOverall.ContainsKey(type)) visibleTypesOverall[type] = 0;
        visibleTypesOverall[type] += c;
    }

    // 3. Analyze CreateDate types and formats
    var datePipeline = new[] {
        new BsonDocument("$group", new BsonDocument {
            { "_id", new BsonDocument("$type", "$CreateDate") },
            { "count", new BsonDocument("$sum", 1) },
            { "samples", new BsonDocument("$push", "$CreateDate") }
        }),
        new BsonDocument("$project", new BsonDocument {
            { "count", 1 },
            { "samples", new BsonDocument("$slice", new BsonArray { "$samples", 5 }) }
        })
    };
    var dateResults = collection.Aggregate<BsonDocument>(datePipeline).ToList();
    foreach (var res in dateResults) {
        var type = res["_id"].ToString();
        var c = res["count"].AsInt32;
        if (!createDateTypesOverall.ContainsKey(type)) createDateTypesOverall[type] = 0;
        createDateTypesOverall[type] += c;

        if (type == "string") {
            foreach (var sample in res["samples"].AsBsonArray) {
                if (sample.IsString) {
                    var s = sample.AsString;
                    var format = GetDateFormat(s);
                    if (!createDateFormats.ContainsKey(format)) createDateFormats[format] = 0;
                    createDateFormats[format]++;
                }
            }
        }
    }
}

string GetDateFormat(string s) {
    if (Regex.IsMatch(s, @"^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}")) return "ISO8601 (YYYY-MM-DDTHH:mm:ss)";
    if (Regex.IsMatch(s, @"^\d{2}/\d{2}/\d{4} \d{2}:\d{2}:\d{2}")) return "DD/MM/YYYY HH:mm:ss";
    if (Regex.IsMatch(s, @"^\d{4}/\d{2}/\d{2} \d{2}:\d{2}:\d{2}")) return "YYYY/MM/DD HH:mm:ss";
    if (Regex.IsMatch(s, @"^\d{1,2}/\d{1,2}/\d{4}")) return "D/M/YYYY";
    return $"Unknown: {s}";
}

Console.WriteLine("\n\n=== FINAL ANALYSIS REPORT ===\n");

Console.WriteLine("Vấn đề #1: ⚠️ _id không nhất quán");
foreach (var kvp in idTypesOverall) {
    Console.WriteLine($"  - Kiểu {kvp.Key}: {kvp.Value} documents");
}

Console.WriteLine("\nVấn đề #2: 🔴 Visible dùng nhiều kiểu dữ liệu");
foreach (var kvp in visibleTypesOverall) {
    Console.WriteLine($"  - Kiểu {kvp.Key}: {kvp.Value} documents");
}

Console.WriteLine("\nVấn đề #3: 🔴 CreateDate không đồng nhất");
Console.WriteLine("  Các kiểu dữ liệu:");
foreach (var kvp in createDateTypesOverall) {
    Console.WriteLine($"    - {kvp.Key}: {kvp.Value} documents");
}
if (createDateFormats.Count > 0) {
    Console.WriteLine("  Các format string tiêu biểu (mẫu):");
    foreach (var kvp in createDateFormats) {
        Console.WriteLine($"    - {kvp.Key}");
    }
}

Console.WriteLine("\n=== END OF REPORT ===");
