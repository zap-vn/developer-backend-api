using Npgsql;
using System;

string connString = "Host=136.118.121.105;Port=5432;Username=postgres;Password=Pg@Secret2026!;Database=zap_ecosystem";
try {
    using (var conn = new NpgsqlConnection(connString))
    {
        conn.Open();
        using (var cmd = new NpgsqlCommand("SELECT column_name FROM information_schema.columns WHERE table_schema = 'platform' AND table_name = 'status_item';", conn))
        {
            using (var reader = cmd.ExecuteReader())
            {
                Console.WriteLine("Columns of platform.status_item:");
                while (reader.Read())
                {
                    Console.WriteLine(reader.GetString(0));
                }
            }
        }
    }
} catch (Exception ex) {
    Console.WriteLine("Error: " + ex.Message);
}
