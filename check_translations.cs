using Npgsql;
using System;

string connString = "Host=136.118.121.105;Port=5432;Username=postgres;Password=Pg@Secret2026!;Database=zap_ecosystem";
try {
    using (var conn = new NpgsqlConnection(connString))
    {
        conn.Open();
        using (var cmd = new NpgsqlCommand("SELECT lookup_id, locale_id, name FROM system.lookup_translations LIMIT 10;", conn))
        {
            using (var reader = cmd.ExecuteReader())
            {
                Console.WriteLine("Data mapping of system.lookup_translations:");
                while (reader.Read())
                {
                    Console.WriteLine($"Type: {reader.GetInt32(0)}, Locale: {reader.GetInt32(1)}, Name: {reader.GetString(2)}");
                }
            }
        }
    }
} catch (Exception ex) {
    Console.WriteLine("Error: " + ex.Message);
}
