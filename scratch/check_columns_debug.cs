using System;
using Npgsql;

class Program
{
    static void Main()
    {
        string connString = "Host=136.118.121.105;Port=5432;Username=postgres;Password=Pg@Secret2026!;Database=zap_ecosystem";
        using var conn = new NpgsqlConnection(connString);
        conn.Open();
        
        string[] tables = { "commerce.location", "platform.status_item", "platform.location_type_item" };
        
        foreach (var table in tables)
        {
            var parts = table.Split('.');
            string schema = parts[0];
            string name = parts[1];
            
            Console.WriteLine($"--- Table: {table} ---");
            using var cmd = new NpgsqlCommand($"SELECT column_name FROM information_schema.columns WHERE table_schema = '{schema}' AND table_name = '{name}'", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine(reader.GetString(0));
            }
            reader.Close();
        }
    }
}
