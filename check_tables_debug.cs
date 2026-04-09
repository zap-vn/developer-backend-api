using Npgsql;
using System;

Console.WriteLine("--- DB CHECK START ---");
string connString = "Host=136.118.121.105;Port=5432;Username=postgres;Password=Pg@Secret2026!;Database=zap_ecosystem";
try {
    using (var conn = new NpgsqlConnection(connString))
    {
        Console.WriteLine("Connecting...");
        conn.Open();
        Console.WriteLine("Connected. Fetching tables...");
        using (var cmd = new NpgsqlCommand("SELECT table_name FROM information_schema.tables WHERE table_schema = 'catalog';", conn))
        {
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Console.WriteLine($"- {reader.GetString(0)}");
                }
            }
        }
    }
} catch (Exception ex) {
    Console.WriteLine("Error: " + ex.Message);
}
Console.WriteLine("--- DB CHECK END ---");
