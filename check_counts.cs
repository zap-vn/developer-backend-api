using System;
using Npgsql;

var connString = "Host=136.118.121.105;Port=5432;Username=postgres;Password=Pg@Secret2026!;Database=zap_ecosystem";
using var conn = new NpgsqlConnection(connString);
conn.Open();

using (var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM pos.location", conn))
{
    var count = cmd.ExecuteScalar();
    Console.WriteLine($"Total rows in pos.location: {count}");
}

using (var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM catalog.custom_unit", conn))
{
    var count = cmd.ExecuteScalar();
    Console.WriteLine($"Total rows in catalog.custom_unit: {count}");
}
