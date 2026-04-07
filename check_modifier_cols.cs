using System;
using System.Data;
using Npgsql;

public class ColumnChecker
{
    public static void Main()
    {
        string connString = "Host=136.118.121.105;Port=5432;Username=postgres;Password=Pg@Secret2026!;Database=zap_ecosystem";
        using (var conn = new NpgsqlConnection(connString))
        {
            conn.Open();
            try
            {
                using (var cmd = new NpgsqlCommand("SELECT column_name FROM information_schema.columns WHERE table_schema = 'catalog' AND table_name = 'modifier_group'", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        Console.WriteLine("Columns in catalog.modifier_group:");
                        while (reader.Read())
                        {
                            Console.WriteLine($"- {reader.GetString(0)}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }
}
