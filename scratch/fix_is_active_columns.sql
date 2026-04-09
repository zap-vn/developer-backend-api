-- Fix Schema inconsistencies and missing columns
-- Ensure platform schema exists
CREATE SCHEMA IF NOT EXISTS platform;

-- 1. status_item (Unified in platform)
DO $$ 
BEGIN
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='platform' AND table_name='status_item' AND column_name='is_active') THEN
        ALTER TABLE platform.status_item ADD COLUMN is_active BOOLEAN DEFAULT TRUE;
    END IF;
END $$;

-- 2. lookups (Unified in platform)
-- Check if lookups exists in platform, if not it might be in system
DO $$ 
BEGIN
    IF EXISTS (SELECT 1 FROM information_schema.tables WHERE table_schema='platform' AND table_name='lookups') THEN
        IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='platform' AND table_name='lookups' AND column_name='is_active') THEN
            ALTER TABLE platform.lookups ADD COLUMN is_active BOOLEAN DEFAULT TRUE;
        END IF;
    ELSIF EXISTS (SELECT 1 FROM information_schema.tables WHERE table_schema='system' AND table_name='lookups') THEN
        IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='system' AND table_name='lookups' AND column_name='is_active') THEN
            ALTER TABLE system.lookups ADD COLUMN is_active BOOLEAN DEFAULT TRUE;
        END IF;
    END IF;
END $$;

-- Ensure system.status_item also has it just in case something still points there
DO $$ 
BEGIN
    IF EXISTS (SELECT 1 FROM information_schema.tables WHERE table_schema='system' AND table_name='status_item') THEN
        IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='system' AND table_name='status_item' AND column_name='is_active') THEN
            ALTER TABLE system.status_item ADD COLUMN is_active BOOLEAN DEFAULT TRUE;
        END IF;
    END IF;
END $$;
