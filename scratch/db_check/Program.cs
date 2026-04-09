using System;
using Npgsql;
using System.IO;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        string connString = "Host=136.118.121.105;Port=5432;Username=postgres;Password=Pg@Secret2026!;Database=zap_ecosystem";
        try {
            using var conn = new NpgsqlConnection(connString);
            conn.Open();
            using var cmd = new NpgsqlCommand("SELECT table_schema, table_name FROM information_schema.tables WHERE table_schema NOT IN ('pg_catalog', 'information_schema')", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine($"{reader.GetString(0)}.{reader.GetString(1)}");
            }
        } catch (Exception ex) {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
}
