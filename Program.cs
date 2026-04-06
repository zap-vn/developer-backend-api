using Npgsql;
using System;

class Program
{
    static void Main()
    {
        var connString = "Host=136.118.121.105;Port=5432;Username=postgres;Password=Pg@Secret2026!;Database=zap_ecosystem";
        using var conn = new NpgsqlConnection(connString);
        try {
            conn.Open();
            Console.WriteLine("Checking platform.status_item...");
            using var cmd = new NpgsqlCommand("SELECT id, code FROM platform.status_item WHERE id = 50", conn);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                Console.WriteLine($"STATUS_CHECK|50|FOUND|{reader.GetString(1)}");
            }
            else
            {
                Console.WriteLine("STATUS_CHECK|50|NOT_FOUND");
                
                // Also check if status 1 exists as it was mentioned in identity_schema_setup
                reader.Close();
                using var cmd2 = new NpgsqlCommand("SELECT id, code FROM platform.status_item WHERE id = 1", conn);
                using var reader2 = cmd2.ExecuteReader();
                if (reader2.Read()) {
                    Console.WriteLine($"STATUS_CHECK|1|FOUND|{reader2.GetString(1)}");
                } else {
                    Console.WriteLine("STATUS_CHECK|1|NOT_FOUND");
                }
            }
        } catch (Exception ex) {
            Console.WriteLine("ERROR: " + ex.Message);
        }
    }
}
