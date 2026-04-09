using System;
using Npgsql;

class Program
{
    static void Main()
    {
        string connString = "Host=136.118.121.105;Port=5432;Username=postgres;Password=Pg@Secret2026!;Database=zap_ecosystem";
        using var conn = new NpgsqlConnection(connString);
        conn.Open();
        string[] tables = { "product_type_item", "status_item", "uom_item", "lookups", "category" };
        foreach (var table in tables)
        {
            using var cmd = new NpgsqlCommand($"SELECT table_schema FROM information_schema.tables WHERE table_name = '{table}'", conn);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                Console.WriteLine($"{table}: {reader.GetString(0)}");
            }
            else
            {
                Console.WriteLine($"{table}: NOT FOUND");
            }
            reader.Close();
        }
    }
}
