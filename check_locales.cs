using Npgsql;
using System;

string connString = "Host=136.118.121.105;Port=5432;Username=postgres;Password=Pg@Secret2026!;Database=zap_ecosystem";
try {
    using (var conn = new NpgsqlConnection(connString))
    {
        conn.Open();
        using (var cmd = new NpgsqlCommand("SELECT id, code, name FROM identity.supported_locale;", conn))
        {
            using (var reader = cmd.ExecuteReader())
            {
                Console.WriteLine("Supported Locales (INT):");
                while (reader.Read())
                {
                    Console.WriteLine($"ID: {reader.GetInt32(0)}, Code: {reader.GetString(1)}, Name: {reader.GetString(2)}");
                }
            }
        }
        Console.WriteLine("\nModern Locales (UUID):");
        using (var cmd = new NpgsqlCommand("SELECT id, code, display_name FROM platform.locale;", conn))
        {
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Console.WriteLine($"ID: {reader.GetGuid(0)}, Code: {reader.GetString(1)}, Name: {reader.GetString(2)}");
                }
            }
        }
    }
} catch (Exception ex) {
    Console.WriteLine("Error: " + ex.Message);
}
