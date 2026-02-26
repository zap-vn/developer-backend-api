using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Analyzer
{
    public class Program
    {
        public static async Task Main()
        {
            var atlasUri = "mongodb+srv://tommy_db_user:Tommy%40123456@cluster0.dcuwhnu.mongodb.net/SinglePoint_en?retryWrites=true&w=majority&appName=Cluster0";
            var client = new MongoClient(atlasUri);
            var database = client.GetDatabase("SinglePoint_en");
            var collection = database.GetCollection<BsonDocument>("Products");
            
            string id = "Product/2997";
            string langCode = "en";
            
            var pipeline = new List<BsonDocument>();
            pipeline.Add(new BsonDocument("$match", new BsonDocument("_id", id)));
            
            // Simulating BuildPipeline Join Translate
            pipeline.Add(new BsonDocument("$lookup", new BsonDocument {
                { "from", "TranslateProduct" },
                { "let", new BsonDocument("mainId", "$_id") },
                { "pipeline", new BsonArray {
                    new BsonDocument("$match", new BsonDocument {
                        { "$expr", new BsonDocument("$and", new BsonArray {
                            new BsonDocument("$eq", new BsonArray { "$ProductGuid", "$$mainId" }),
                            new BsonDocument("$eq", new BsonArray { "$Code", langCode })
                        })}
                    })
                }},
                { "as", "Translations" }
            }));
            pipeline.Add(new BsonDocument("$addFields", new BsonDocument("Translation", new BsonDocument("$arrayElemAt", new BsonArray { "$Translations", 0 }))));

            // Simulating BuildPipeline AddFields for Name
            var options = new BsonArray {
                "$Name_en",
                "$name_en",
                "$Translation.Title",
                "$Locales.en.Name",
                "$Name_vi",
                "$Name",
                "$Title",
                BsonNull.Value
            };
            pipeline.Add(new BsonDocument("$addFields", new BsonDocument {
                { "MappedName", new BsonDocument("$ifNull", options) },
                { "RawTranslation", "$Translation" }
            }));

            Console.Error.WriteLine($"Running pipeline for {id}...");
            var result = await collection.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();

            if (result != null) {
                Console.Error.WriteLine("RESULT:");
                Console.Error.WriteLine(result.ToJson(new MongoDB.Bson.IO.JsonWriterSettings { Indent = true }));
            } else {
                Console.Error.WriteLine("Document not found!");
            }
        }
    }
}
