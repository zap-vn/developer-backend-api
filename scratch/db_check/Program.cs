using System;
using Npgsql;

class Program
{
    static void Main()
    {
        string connString = "Host=136.118.121.105;Port=5432;Username=postgres;Password=Pg@Secret2026!;Database=zap_ecosystem";
        try {
            using var conn = new NpgsqlConnection(connString);
            conn.Open();
            using var cmd = new NpgsqlCommand("SELECT nspname FROM pg_namespace WHERE nspname NOT LIKE 'pg_%' AND nspname != 'information_schema'", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) Console.WriteLine("SCHEMA: " + reader.GetString(0));
        } catch (Exception ex) { Console.WriteLine("ERROR: " + ex.Message); }
    }
}
