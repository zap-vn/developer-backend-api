
using System;
using Npgsql;
using System.IO;

class DbFix {
    static void Main() {
        string connString = "Host=136.118.121.105;Port=5432;Username=postgres;Password=Pg@Secret2026!;Database=zap_ecosystem";
        try {
            using (var conn = new NpgsqlConnection(connString)) {
                conn.Open();
                Console.WriteLine("Connected to DB");
                string sql = File.ReadAllText(@"D:\PROJECTS\4_2026\01042026\scratch\fix_is_active_columns.sql");
                using (var cmd = new NpgsqlCommand(sql, conn)) {
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("SQL Fix Applied Successfully");
                }
            }
        } catch (Exception ex) {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
}
