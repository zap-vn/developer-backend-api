using System;
using Npgsql;

class Program
{
    static void Main()
    {
        string connString = "Host=136.118.121.105;Port=5432;Username=postgres;Password=Pg@Secret2026!;Database=zap_ecosystem";
        using var conn = new NpgsqlConnection(connString);
        conn.Open();
        using var cmd = new NpgsqlCommand("SELECT column_name, data_type FROM information_schema.columns WHERE table_schema = 'system' AND table_name = 'status_item'", conn);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            Console.WriteLine($"{reader.GetString(0)}: {reader.GetString(1)}");
        }
    }
}
