using Npgsql;
using System;

var connectionString = "Host=136.118.121.105;Port=5432;Username=postgres;Password=Pg@Secret2026!;Database=zap_ecosystem";

using var conn = new NpgsqlConnection(connectionString);
conn.Open();

string[] tables = { "dining_option", "dining_option_translation", "dining_option_location_link" };

foreach (var table in tables)
{
    Console.WriteLine($"\n--- Table: {table} ---");
    var cmd = new NpgsqlCommand($@"
        SELECT column_name, data_type, is_nullable
        FROM information_schema.columns 
        WHERE table_schema = 'platform' AND table_name = '{table}'", conn);
    
    using var reader = cmd.ExecuteReader();
    bool found = false;
    while (reader.Read())
    {
        found = true;
        Console.WriteLine($"{reader["column_name"]} | {reader["data_type"]} | {reader["is_nullable"]}");
    }
    if (!found) Console.WriteLine("Not found in 'platform' schema.");
    reader.Close();
}

// Try 'pos' schema for the link table just in case
Console.WriteLine("\n--- Checking 'pos.dining_option_location_link' ---");
var cmdLink = new NpgsqlCommand(@"
    SELECT column_name, data_type 
    FROM information_schema.columns 
    WHERE table_schema = 'pos' AND table_name = 'dining_option_location_link'", conn);
using var readerLink = cmdLink.ExecuteReader();
while (readerLink.Read())
{
    Console.WriteLine($"{readerLink["column_name"]} | {readerLink["data_type"]}");
}
readerLink.Close();
