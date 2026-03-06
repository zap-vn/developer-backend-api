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
        string databaseName = "SinglePoint_en";
        StringBuilder sb = new StringBuilder();
        
        try
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            var indexCol = database.GetCollection<BsonDocument>("ManagementIndex");
            var customerCol = database.GetCollection<BsonDocument>("Customer");
            
            sb.AppendLine("--- ManagementIndex ---");
            var indexes = await indexCol.Find(new BsonDocument()).ToListAsync();
            foreach (var idx in indexes) sb.AppendLine(idx.ToJson());
            
            sb.AppendLine("\n--- Last 5 Customers ---");
            var customers = await customerCol.Find(new BsonDocument()).Sort(Builders<BsonDocument>.Sort.Descending("_id")).Limit(5).ToListAsync();
            foreach (var c in customers) sb.AppendLine(c.ToJson());
            
            if (indexes.Count == 0 && customers.Count == 0) sb.AppendLine("Database is empty or collection names are wrong.");
            
            File.WriteAllText("final_check_output.txt", sb.ToString());
            Console.WriteLine("Diagnostics written to final_check_output.txt");
        }
        catch (Exception ex)
        {
            File.WriteAllText("final_check_output.txt", "Error: " + ex.Message);
        }
    }
}
