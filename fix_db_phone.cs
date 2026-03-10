using MongoDB.Driver;
using MongoDB.Bson;
using System;
using System.Threading.Tasks;

public class DbFix
{
    public static async Task Main()
    {
        var connectionString = "mongodb://tommy_db_user:Tommy123456@ac-ewrdepk-shard-00-00.dcuwhnu.mongodb.net:27017,ac-ewrdepk-shard-00-01.dcuwhnu.mongodb.net:27017,ac-ewrdepk-shard-00-02.dcuwhnu.mongodb.net:27017/SinglePoint_en?ssl=true&authSource=admin&retryWrites=true&w=majority";
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase("SinglePoint_en");
        var collection = database.GetCollection<BsonDocument>("Customer");

        // Try to find a user and update their phone number
        var filterEmail = Builders<BsonDocument>.Filter.Eq("Email", "tommy@zap.vn");
        var user = await collection.Find(filterEmail).FirstOrDefaultAsync();

        if (user != null)
        {
            var update = Builders<BsonDocument>.Update.Set("Phone", "0919136010");
            await collection.UpdateOneAsync(filterEmail, update);
            Console.WriteLine("SUCCESS: Updated phone for " + user["Email"]);
        }
        else
        {
            // Find ANY user
            var anyUser = await collection.Find(Builders<BsonDocument>.Filter.Empty).FirstOrDefaultAsync();
            if (anyUser != null) {
                var update = Builders<BsonDocument>.Update.Set("Phone", "0919136010");
                await collection.UpdateOneAsync(Builders<BsonDocument>.Filter.Eq("_id", anyUser["_id"]), update);
                Console.WriteLine("SUCCESS: Updated phone for " + anyUser["Email"]);
            } else {
                Console.WriteLine("ERROR: No users found.");
            }
        }
    }
}
