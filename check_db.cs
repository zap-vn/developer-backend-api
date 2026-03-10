using MongoDB.Driver;
using System;
using System.Threading.Tasks;

public class DbCheck
{
    public static async Task Main()
    {
        var connectionString = "mongodb://tommy_db_user:Tommy123456@ac-ewrdepk-shard-00-00.dcuwhnu.mongodb.net:27017,ac-ewrdepk-shard-00-01.dcuwhnu.mongodb.net:27017,ac-ewrdepk-shard-00-02.dcuwhnu.mongodb.net:27017/SinglePoint_en?ssl=true&authSource=admin&retryWrites=true&w=majority";
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase("SinglePoint_en");
        var collection = database.GetCollection<MongoDB.Bson.BsonDocument>("Customer");

        var phone = "0919136010";
        var filter = Builders<MongoDB.Bson.BsonDocument>.Filter.Eq("Phone", phone);
        var user = await collection.Find(filter).FirstOrDefaultAsync();

        if (user != null)
        {
            Console.WriteLine($"Found user: {user["Email"]}");
        }
        else
        {
            Console.WriteLine("User not found with phone: " + phone);
            
            // List some users to see schema
            var anyUser = await collection.Find(Builders<MongoDB.Bson.BsonDocument>.Filter.Empty).Limit(3).ToListAsync();
            foreach(var u in anyUser) {
                Console.WriteLine($"Email: {u.GetValue("Email", "N/A")}, Phone: {u.GetValue("Phone", "N/A")}");
            }
        }
    }
}
