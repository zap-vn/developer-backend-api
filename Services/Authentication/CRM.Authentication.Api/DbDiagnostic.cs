using System;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Threading.Tasks;

public class DbDiagnostic
{
    public static async Task Run(string connString)
    {
        Console.Error.WriteLine("--- Table: identity.tenant_node Constraints ---");
        using var conn = new NpgsqlConnection(connString);
        await conn.OpenAsync();

        using (var cmd = new NpgsqlCommand(@"
            SELECT
                conname AS constraint_name,
                pg_get_constraintdef(c.oid) AS constraint_definition
            FROM
                pg_constraint c
            JOIN
                pg_namespace n ON n.oid = c.connamespace
            WHERE
                n.nspname = 'identity' 
                AND c.conrelid = 'identity.tenant_node'::regclass
        ", conn))
        {
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                Console.Error.WriteLine($"Constraint: {reader["constraint_name"]}");
                Console.Error.WriteLine($"Definition: {reader["constraint_definition"]}\n");
            }
        }

        Console.Error.WriteLine("--- Checking identity.locale table existence ---");
        using (var cmd = new NpgsqlCommand(@"
            SELECT EXISTS (
               SELECT FROM information_schema.tables 
               WHERE  table_schema = 'identity'
               AND    table_name   = 'locale'
            );
        ", conn))
        {
            var exists = (bool)await cmd.ExecuteScalarAsync();
            Console.Error.WriteLine($"Table identity.locale exists: {exists}");
            
            if (exists) {
                using var cmd2 = new NpgsqlCommand("SELECT id, name FROM identity.locale LIMIT 10", conn);
                using var reader2 = await cmd2.ExecuteReaderAsync();
                while (await reader2.ReadAsync()) {
                    Console.Error.WriteLine($"Locale: {reader2["id"]} - {reader2["name"]}");
                }
            }
        }
        
        Console.Error.WriteLine("--- Checking identity.language table existence ---");
        using (var cmd = new NpgsqlCommand(@"
            SELECT EXISTS (
               SELECT FROM information_schema.tables 
               WHERE  table_schema = 'identity'
               AND    table_name   = 'language'
            );
        ", conn))
        {
            var exists = (bool)await cmd.ExecuteScalarAsync();
            Console.Error.WriteLine($"Table identity.language exists: {exists}");
        }
    }
}
