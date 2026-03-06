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
            var client = new MongoClient(connectionString);
            sb.AppendLine("Listing all databases:");
            using var cursor = await client.ListDatabaseNamesAsync();
            while (await cursor.MoveNextAsync())
            {
                foreach (var dbName in cursor.Current)
                {
                    sb.AppendLine($"- {dbName}");
                    var db = client.GetDatabase(dbName);
                    var collections = await db.ListCollectionNamesAsync();
                    while (await collections.MoveNextAsync())
                    {
                        foreach (var colName in collections.Current)
                        {
                            sb.AppendLine($"  -- {colName}");
                        }
                    }
                }
            }
            
            File.WriteAllText("db_list.txt", sb.ToString());
            Console.WriteLine("Done.");
        }
        catch (Exception ex)
        {
            File.WriteAllText("db_list.txt", "Error: " + ex.Message + "\n" + ex.StackTrace);
        }
    }
}
