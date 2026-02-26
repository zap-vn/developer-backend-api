using MongoDB.Driver;
using System;

try {
    var connectionString = "mongodb://172.16.10.153:27017/?retryWrites=false&loadBalanced=false&connectTimeoutMS=5000";
    var client = new MongoClient(connectionString);
    var dbs = client.ListDatabaseNames().ToList();
    Console.WriteLine("DBS: " + string.Join(", ", dbs));
} catch (Exception ex) {
    Console.WriteLine("ERROR: " + ex.Message);
}
