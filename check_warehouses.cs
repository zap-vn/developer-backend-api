using System;
using System.Linq;
using Npgsql;
using System.Data;

var connString = "Host=136.118.121.105;Port=5432;Username=postgres;Password=Pg@Secret2026!;Database=zap_ecosystem";
using var conn = new NpgsqlConnection(connString);
conn.Open();

Console.WriteLine("Listing all warehouses in pos.location:");
using (var cmd = new NpgsqlCommand("SELECT id, name, tenant_id FROM pos.location LIMIT 10", conn))
using (var reader = cmd.ExecuteReader())
{
    while (reader.Read())
    {
        Console.WriteLine($"ID: {reader["id"]}, Name: {reader["name"]}, Tenant: {reader["tenant_id"]}");
    }
}
