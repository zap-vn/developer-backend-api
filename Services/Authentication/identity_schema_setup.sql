-- ======================================================
-- ZAP 2026 - Identity Schema: Create Tables
-- Target: zap_ecosystem database
-- Schema: identity (theo blueprint AUTHENTICATION_API_GUIDE.md)
-- ======================================================

-- Tạo schema nếu chưa có
CREATE SCHEMA IF NOT EXISTS identity;

-- ======================================================
-- 1. identity.tenant_node - Dành cho Merchant/Brand/Store
-- ======================================================
CREATE TABLE IF NOT EXISTS identity.tenant_node (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    parent_id       UUID REFERENCES identity.tenant_node(id),
    node_code       VARCHAR(64) NOT NULL UNIQUE,
    legacy_id       VARCHAR(128),
    tier_level      INT NOT NULL DEFAULT 2,  -- 1: Root, 2: Brand, 3: Store
    name            VARCHAR(255) NOT NULL,
    slug            VARCHAR(128) NOT NULL UNIQUE,
    locale_id       INT DEFAULT 1,
    status_id       INT DEFAULT 50,          -- 50: ACTIVE
    address_line_1  VARCHAR(255),
    timezone        VARCHAR(64) DEFAULT 'Asia/Ho_Chi_Minh',
    created_at      TIMESTAMPTZ DEFAULT now(),
    updated_at      TIMESTAMPTZ DEFAULT now()
);

-- ======================================================
-- 2. identity.user - Dành cho SuperAdmin, Merchant, Nhân viên
-- (Internal Access, KHÔNG dùng cho khách hàng)
-- ======================================================
CREATE TABLE IF NOT EXISTS identity.user (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id       UUID REFERENCES identity.tenant_node(id),
    legacy_id       VARCHAR(128),
    username        VARCHAR(128) NOT NULL UNIQUE,
    email           VARCHAR(128) NOT NULL UNIQUE,
    password_hash   TEXT NOT NULL DEFAULT '',
    full_name       VARCHAR(255) NOT NULL DEFAULT '',
    status_id       INT DEFAULT 1,
    created_at      TIMESTAMPTZ DEFAULT now(),
    updated_at      TIMESTAMPTZ DEFAULT now()
);

-- ======================================================
-- 3. identity.otp - OTP cho xác thực
-- ======================================================
CREATE TABLE IF NOT EXISTS identity.otp (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    email           VARCHAR(128),
    phone           VARCHAR(32),
    otp_code        VARCHAR(16) NOT NULL,
    purpose         VARCHAR(64),
    customer_id     VARCHAR(128),
    created_at      TIMESTAMPTZ DEFAULT now(),
    expired_at      TIMESTAMPTZ,
    verified_at     TIMESTAMPTZ
);

-- ======================================================
-- 4. identity.password_reset - Reset mật khẩu
-- ======================================================
CREATE TABLE IF NOT EXISTS identity.password_reset (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_guid       VARCHAR(128),
    email           VARCHAR(128) NOT NULL DEFAULT '',
    phone           VARCHAR(32),
    method          VARCHAR(16),
    channel         VARCHAR(16),
    otp_hash        TEXT,
    token           TEXT NOT NULL DEFAULT '',
    confirm_token   TEXT,
    attempts        INT DEFAULT 0,
    is_used         BOOLEAN DEFAULT false,
    expired_at      TIMESTAMPTZ,
    created_at      TIMESTAMPTZ DEFAULT now()
);

-- ======================================================
-- 5. identity.email_setting - Cấu hình email theo Merchant
-- ======================================================
CREATE TABLE IF NOT EXISTS identity.email_setting (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    customer_guid   VARCHAR(128),
    smtp_host       VARCHAR(255),
    smtp_port       INT,
    smtp_user       VARCHAR(255),
    smtp_pass       TEXT,
    from_email      VARCHAR(255),
    from_name       VARCHAR(255),
    created_at      TIMESTAMPTZ DEFAULT now()
);

-- ======================================================
-- 6. identity.system_config - Cấu hình hệ thống
-- ======================================================
CREATE TABLE IF NOT EXISTS identity.system_config (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    key             VARCHAR(255) NOT NULL UNIQUE,
    value           TEXT NOT NULL DEFAULT '',
    description     TEXT
);

-- ======================================================
-- 7. identity.system_error - Log lỗi hệ thống
-- ======================================================
CREATE TABLE IF NOT EXISTS identity.system_error (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    message         TEXT,
    detail          TEXT,
    source          VARCHAR(255),
    user_id         VARCHAR(128),
    merchant_name   VARCHAR(255),
    created_at      TIMESTAMPTZ DEFAULT now()
);

-- ======================================================
-- Sequence cho Merchant ID (dùng bởi GetNextSequenceAsync)
-- ======================================================
CREATE SEQUENCE IF NOT EXISTS identity.customer_id START 1;

-- ======================================================
-- Indexes để tăng tốc query
-- ======================================================
CREATE INDEX IF NOT EXISTS idx_user_email ON identity.user(email);
CREATE INDEX IF NOT EXISTS idx_user_username ON identity.user(username);
CREATE INDEX IF NOT EXISTS idx_user_tenant_id ON identity.user(tenant_id);
CREATE INDEX IF NOT EXISTS idx_tenant_slug ON identity.tenant_node(slug);
CREATE INDEX IF NOT EXISTS idx_otp_customer_purpose ON identity.otp(customer_id, purpose);
CREATE INDEX IF NOT EXISTS idx_password_reset_token ON identity.password_reset(token);

-- Done!
SELECT 'identity schema setup complete' AS status;
