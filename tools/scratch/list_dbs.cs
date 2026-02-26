using MongoDB.Driver;
using System;
using System.Threading.Tasks;

public class ListDbs
{
    public static async Task Main()
    {
        try {
            var client = new MongoClient("mongodb://172.16.10.153:27017");
            var dbs = await client.ListDatabaseNamesAsync();
            Console.WriteLine("Databases found:");
            await dbs.ForEachAsync(db => Console.WriteLine($" - {db}"));
        } catch (Exception ex) {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
