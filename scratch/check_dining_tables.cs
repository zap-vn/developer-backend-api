using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Data;

var connectionString = "Host=136.118.121.105;Port=5432;Username=postgres;Password=Pg@Secret2026!;Database=zap_ecosystem";

using var conn = new NpgsqlConnection(connectionString);
conn.Open();

Console.WriteLine("Checking tables in 'platform' schema:");
var cmd = new NpgsqlCommand("SELECT table_name FROM information_schema.tables WHERE table_schema = 'platform'", conn);
using var reader = cmd.ExecuteReader();
while (reader.Read())
{
    Console.WriteLine($"- {reader["table_name"]}");
}
reader.Close();

Console.WriteLine("\nChecking columns for 'dining_option':");
var cmd2 = new NpgsqlCommand("SELECT column_name, data_type FROM information_schema.columns WHERE table_schema = 'platform' AND table_name = 'dining_option'", conn);
try {
    using var reader2 = cmd2.ExecuteReader();
    while (reader2.Read())
    {
        Console.WriteLine($"- {reader2["column_name"]} ({reader2["data_type"]})");
    }
} catch (Exception ex) {
    Console.WriteLine($"Error: {ex.Message}");
}
