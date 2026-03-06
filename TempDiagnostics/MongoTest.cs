using MongoDB.Driver;
using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        string connectionString = "mongodb+srv://tommy_db_user:Tommy123456@cluster0.dcuwhnu.mongodb.net/?appName=Cluster0";
        string databaseName = "SinglePoint_en";
        
        try
        {
            Console.WriteLine("Connecting to MongoDB...");
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            
            Console.WriteLine("Attempting to list collection names...");
            var collections = await database.ListCollectionNames().ToListAsync();
            
            Console.WriteLine("Successfully connected!");
            Console.WriteLine("Collections found: " + string.Join(", ", collections));
        }
        catch (Exception ex)
        {
            Console.WriteLine("FAILED to connect to MongoDB.");
            Console.WriteLine("Error: " + ex.Message);
            if (ex.InnerException != null)
            {
                Console.WriteLine("Inner Error: " + ex.InnerException.Message);
            }
        }
    }
}
