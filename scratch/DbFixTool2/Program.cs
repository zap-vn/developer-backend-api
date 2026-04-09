
using System;
using Npgsql;

class Program {
    static void Main() {
        string connString = "Host=136.118.121.105;Port=5432;Username=postgres;Password=Pg@Secret2026!;Database=zap_ecosystem";
        try {
            using (var conn = new NpgsqlConnection(connString)) {
                conn.Open();
                Console.WriteLine("DB Connected");
                string sql = @"
CREATE SCHEMA IF NOT EXISTS platform;
DO $$ BEGIN IF EXISTS (SELECT 1 FROM information_schema.tables WHERE table_schema='system' AND table_name='status_item') THEN ALTER TABLE system.status_item SET SCHEMA platform; END IF; END $$;
DO $$ BEGIN IF EXISTS (SELECT 1 FROM information_schema.tables WHERE table_schema='system' AND table_name='status_item_translation') THEN ALTER TABLE system.status_item_translation SET SCHEMA platform; END IF; END $$;
DO $$ BEGIN IF EXISTS (SELECT 1 FROM information_schema.tables WHERE table_schema='system' AND table_name='lookups') THEN ALTER TABLE system.lookups SET SCHEMA platform; END IF; END $$;
DO $$ BEGIN IF EXISTS (SELECT 1 FROM information_schema.tables WHERE table_schema='system' AND table_name='lookup_translations') THEN ALTER TABLE system.lookup_translations SET SCHEMA platform; END IF; END $$;

-- Check and add columns
DO $$ BEGIN IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='platform' AND table_name='status_item' AND column_name='is_active') THEN ALTER TABLE platform.status_item ADD COLUMN is_active BOOLEAN DEFAULT TRUE; END IF; END $$;
DO $$ BEGIN IF EXISTS (SELECT 1 FROM information_schema.tables WHERE table_schema='platform' AND table_name='lookups') THEN IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='platform' AND table_name='lookups' AND column_name='is_active') THEN ALTER TABLE platform.lookups ADD COLUMN is_active BOOLEAN DEFAULT TRUE; END IF; END IF; END $$;
";
                using (var cmd = new NpgsqlCommand(sql, conn)) {
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("SQL Move and Fix Applied Successfully");
                }
            }
        } catch (Exception ex) {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
}
