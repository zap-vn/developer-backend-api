using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace DataMigrator;

class Program
{
    static async Task Main(string[] args)
    {
        try {
            var atlasUri = "mongodb+srv://tommy_db_user:Tommy123456@cluster0.dcuwhnu.mongodb.net/";
            var client = new MongoClient(atlasUri);
            
            var localUri = "mongodb://172.16.10.153:27017/";
            var localClient = new MongoClient(localUri);
            
            string[] enCols = { "GroupEmployee" };
            foreach (var col in enCols)
            {
                var localCol = localClient.GetDatabase("SinglePoint_en").GetCollection<BsonDocument>(col);
                var atlasCol = client.GetDatabase("SinglePoint_en").GetCollection<BsonDocument>(col);
                var docs = await localCol.Find(new BsonDocument()).ToListAsync();
                foreach (var doc in docs)
                {
                    await atlasCol.ReplaceOneAsync(new BsonDocument("_id", doc["_id"]), doc, new ReplaceOptions { IsUpsert = true });
                }
                Console.WriteLine($"✅ Migrated SinglePoint_en.{col}");
            }
        } catch (Exception ex) {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
