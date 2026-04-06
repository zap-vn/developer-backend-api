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
            using var cmd = new NpgsqlCommand("SELECT email, password_hash, status_id FROM identity.user WHERE email = 'user_24700_4@gmail.com'", conn);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                Console.WriteLine($"USER_DATA_START|{reader.GetString(0)}|{reader.GetString(1)}|{reader.GetInt32(2)}|USER_DATA_END");
            }
            else
            {
                Console.WriteLine("USER_NOT_FOUND");
            }
        } catch (Exception ex) {
            Console.WriteLine("ERROR: " + ex.Message);
        }
    }
}
