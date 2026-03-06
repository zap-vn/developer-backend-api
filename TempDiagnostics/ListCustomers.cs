using MongoDB.Driver;
using MongoDB.Bson;
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
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            var collection = database.GetCollection<BsonDocument>("Customer");
            
            Console.WriteLine("Listing all customers in database '" + databaseName + "', collection 'Customer':");
            var customers = await collection.Find(new BsonDocument()).ToListAsync();
            
            foreach (var doc in customers)
            {
                Console.WriteLine(doc.ToJson());
            }
            
            if (customers.Count == 0)
            {
                Console.WriteLine("No customers found.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
}
