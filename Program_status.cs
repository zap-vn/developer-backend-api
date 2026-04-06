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
                Console.WriteLine($"Status 50 found: {reader.GetString(1)}");
            }
            else
            {
                Console.WriteLine("Status 50 NOT FOUND in platform.status_item!");
            }
        } catch (Exception ex) {
            Console.WriteLine("ERROR: " + ex.Message);
        }
    }
}
