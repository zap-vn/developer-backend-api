using MongoDB.Driver;
using MongoDB.Bson;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        string connectionString = "mongodb+srv://tommy_db_user:Tommy123456@cluster0.dcuwhnu.mongodb.net/?appName=Cluster0";
        StringBuilder sb = new StringBuilder();
        try
        {
            sb.AppendLine("Testing SRV: " + connectionString);
            var client = new MongoClient(connectionString);
            var result = await client.GetDatabase("admin").RunCommandAsync<BsonDocument>(new BsonDocument("isMaster", 1));
            sb.AppendLine("SRV result: " + result.ToJson());
        }
        catch (Exception ex)
        {
            sb.AppendLine("SRV Error: " + ex.Message);
            try {
                sb.AppendLine("\nTesting Direct Shard-00-00...");
                var directClient = new MongoClient("mongodb://tommy_db_user:Tommy123456@ac-ewrdepk-shard-00-00.dcuwhnu.mongodb.net:27017/?ssl=true&authSource=admin");
                var resultDirect = await directClient.GetDatabase("admin").RunCommandAsync<BsonDocument>(new BsonDocument("isMaster", 1));
                sb.AppendLine("Direct result: " + resultDirect.ToJson());
                if (resultDirect.Contains("setName")) {
                    sb.AppendLine("ReplicaSet: " + resultDirect["setName"]);
                }
            } catch (Exception ex2) {
                sb.AppendLine("Direct Error: " + ex2.Message);
            }
        }
        File.WriteAllText("rs_check_output.txt", sb.ToString());
        Console.WriteLine("Check complete. rs_check_output.txt updated.");
    }
}
