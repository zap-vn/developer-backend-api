using Npgsql;
using System;

string connString = "Host=136.118.121.105;Port=5432;Username=postgres;Password=Pg@Secret2026!;Database=zap_ecosystem";
try {
    using (var conn = new NpgsqlConnection(connString))
    {
        conn.Open();
        Console.WriteLine("Connection opened.");
        using (var cmd = new NpgsqlCommand("SELECT table_schema, table_name, column_name FROM information_schema.columns WHERE table_name = 'uom_item';", conn))
        {
            using (var reader = cmd.ExecuteReader())
            {
                Console.WriteLine("Columns of uom_item:");
                while (reader.Read())
                {
                    Console.WriteLine($"{reader.GetString(0)}.{reader.GetString(1)} : {reader.GetString(2)}");
                }
            }
        }
    }
} catch (Exception ex) {
    Console.WriteLine("Error: " + ex.Message);
}
