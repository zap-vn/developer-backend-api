using CRM.BuildingBlocks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using CRM.Authentication.Application;
using CRM.Authentication.Infrastructure;
using CRM.BuildingBlocks.Middleware;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = new CRM.BuildingBlocks.Serialization.CrmSnakeCaseNamingPolicy();
        options.JsonSerializerOptions.DictionaryKeyPolicy = new CRM.BuildingBlocks.Serialization.CrmSnakeCaseNamingPolicy();
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLocalization();
builder.Services.AddMemoryCache();
builder.Services.AddBackgroundQueue(100);
builder.Services.AddHttpClient();
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

// TTL Application Services
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddSingleton<CRM.Authentication.Infrastructure.Security.OtpService>();

// Authentication setup
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"] ?? "a_very_secret_default_key_at_least_32_chars_long_1234567890"))
        };
    });

builder.Services.AddBuildingBlocks();
builder.Services.AddHealthChecks(); // Register health checks

try {
    var app = builder.Build();

    app.UseMiddleware<LocalizationMiddleware>();
    app.UseMiddleware<ExceptionMiddleware>();

    app.UseResponseCompression();
    app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("v1/swagger.json", "Login API v1");
            c.RoutePrefix = "swagger"; // Explicitly set or keep empty, but let's see why it failed.
        });

    app.UseRouting();

    app.UseCors("AllowAll");

    // app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();

    // Duplicate LocalizationMiddleware removed
    // ✅ Auto-initialize Database Schema
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<CRM.Authentication.Infrastructure.Persistence.PostgresDbContext>();
        try
        {
            // Block 1: Schema & Sequences
            await dbContext.Database.ExecuteSqlRawAsync(@"
                CREATE SCHEMA IF NOT EXISTS identity;
                CREATE SEQUENCE IF NOT EXISTS identity.customer_id START 1;
            ");

            // Block 2: Fundamental Tables
            await dbContext.Database.ExecuteSqlRawAsync(@"
                CREATE TABLE IF NOT EXISTS identity.locale (
                    id   INT PRIMARY KEY,
                    name VARCHAR(128) NOT NULL
                );
                INSERT INTO identity.locale (id, name) VALUES (0, 'Unknown'), (1, 'vi-VN'), (2, 'en-US'), (3, 'th-TH'), (4, 'id-ID'), (5, 'ms-MY'), (6, 'zh-CN'), (7, 'zh-TW'), (8, 'ja-JP'), (9, 'ko-KR'), (10, 'lo-LA') ON CONFLICT (id) DO NOTHING;

                CREATE TABLE IF NOT EXISTS identity.language (
                    id   INT PRIMARY KEY,
                    name VARCHAR(128) NOT NULL
                );
                INSERT INTO identity.language (id, name) VALUES (0, 'Unknown'), (1, 'Vietnamese'), (2, 'English'), (3, 'Thai'), (4, 'Indonesian'), (5, 'Malay'), (6, 'Chinese'), (7, 'Taiwanese'), (8, 'Japanese'), (9, 'Korean'), (10, 'Lao') ON CONFLICT (id) DO NOTHING;

                CREATE TABLE IF NOT EXISTS identity.status (
                    id   INT PRIMARY KEY,
                    name VARCHAR(128) NOT NULL
                );
                INSERT INTO identity.status (id, name) VALUES (1, 'PENDING'), (50, 'ACTIVE'), (99, 'SUSPENDED'), (9001, 'ACTIVE_USER'), (9002, 'LOCKED'), (9003, 'PENDING_MFA'), (9101, 'ACTIVE_KEY') ON CONFLICT (id) DO NOTHING;
            ");

            // Block 3: Tenant & User Tables (with constraint fixes)
            await dbContext.Database.ExecuteSqlRawAsync(@"
                CREATE TABLE IF NOT EXISTS identity.tenant_node (
                    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                    parent_id       UUID,
                    node_code       VARCHAR(64) NOT NULL,
                    legacy_id       VARCHAR(128),
                    tier_level      INT NOT NULL DEFAULT 2,
                    name            VARCHAR(255) NOT NULL,
                    slug            VARCHAR(128) NOT NULL,
                    locale_id       INT DEFAULT 1,
                    status_id       INT DEFAULT 50,
                    address_line_1  VARCHAR(255),
                    timezone        VARCHAR(64) DEFAULT 'Asia/Ho_Chi_Minh',
                    created_at      TIMESTAMPTZ DEFAULT now(),
                    updated_at      TIMESTAMPTZ DEFAULT now(),
                    CONSTRAINT uq_tenant_node_code UNIQUE (node_code),
                    CONSTRAINT uq_tenant_slug UNIQUE (slug),
                    CONSTRAINT fk_tenant_node_parent FOREIGN KEY (parent_id) REFERENCES identity.tenant_node(id)
                );
                
                DO $$ BEGIN
                    IF EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'tenant_node_locale_id_fkey') THEN
                        ALTER TABLE identity.tenant_node DROP CONSTRAINT tenant_node_locale_id_fkey;
                    END IF;
                    ALTER TABLE identity.tenant_node ADD CONSTRAINT tenant_node_locale_id_fkey FOREIGN KEY (locale_id) REFERENCES identity.locale(id);
                    
                    IF EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'tenant_node_status_id_fkey') THEN
                        ALTER TABLE identity.tenant_node DROP CONSTRAINT tenant_node_status_id_fkey;
                    END IF;
                    ALTER TABLE identity.tenant_node ADD CONSTRAINT tenant_node_status_id_fkey FOREIGN KEY (status_id) REFERENCES identity.status(id);
                END $$;

                CREATE TABLE IF NOT EXISTS identity.user (
                    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                    tenant_id       UUID REFERENCES identity.tenant_node(id),
                    legacy_id       VARCHAR(128) UNIQUE,
                    username        VARCHAR(128) UNIQUE NOT NULL,
                    email           VARCHAR(128) UNIQUE NOT NULL,
                    password_hash   TEXT NOT NULL,
                    full_name       VARCHAR(255) NOT NULL DEFAULT 'SYSTEM_USER',
                    status_id       INT DEFAULT 9001,
                    created_at      TIMESTAMPTZ DEFAULT now(),
                    updated_at      TIMESTAMPTZ DEFAULT now()
                );

                DO $$ BEGIN
                    IF EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'user_status_id_fkey') THEN
                        ALTER TABLE identity.user DROP CONSTRAINT user_status_id_fkey;
                    END IF;
                    ALTER TABLE identity.user ADD CONSTRAINT user_status_id_fkey FOREIGN KEY (status_id) REFERENCES identity.status(id);
                END $$;
            ");

            // Block 4: Other tables
            await dbContext.Database.ExecuteSqlRawAsync(@"
                CREATE TABLE IF NOT EXISTS identity.user_role_scope (
                    id          UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                    user_id     UUID REFERENCES identity.user(id),
                    scope_type  VARCHAR(16) DEFAULT 'GLOBAL',
                    scope_id    UUID,
                    assigned_at TIMESTAMPTZ DEFAULT now()
                );

                CREATE TABLE IF NOT EXISTS identity.otp (
                    id          UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                    email       VARCHAR(128),
                    phone       VARCHAR(32),
                    otp_code    VARCHAR(16) NOT NULL,
                    purpose     VARCHAR(64),
                    customer_id VARCHAR(128),
                    created_at  TIMESTAMPTZ DEFAULT now(),
                    expired_at  TIMESTAMPTZ,
                    verified_at TIMESTAMPTZ
                );

                CREATE TABLE IF NOT EXISTS identity.password_reset (
                    id            UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                    user_guid     VARCHAR(128),
                    email         VARCHAR(128) NOT NULL DEFAULT '',
                    phone         VARCHAR(32),
                    method        VARCHAR(16),
                    channel       VARCHAR(16),
                    otp_hash      TEXT,
                    token         TEXT NOT NULL DEFAULT '',
                    confirm_token TEXT,
                    attempts      INT DEFAULT 0,
                    is_used       BOOLEAN DEFAULT false,
                    expired_at    TIMESTAMPTZ,
                    created_at    TIMESTAMPTZ DEFAULT now()
                );

                CREATE TABLE IF NOT EXISTS identity.email_setting (
                    id            UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                    customer_guid VARCHAR(128),
                    smtp_host     VARCHAR(255),
                    smtp_port     INT,
                    smtp_user     VARCHAR(255),
                    smtp_pass     TEXT,
                    from_email    VARCHAR(255),
                    from_name     VARCHAR(255),
                    created_at    TIMESTAMPTZ DEFAULT now()
                );

                CREATE TABLE IF NOT EXISTS identity.system_config (
                    id          UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                    key         VARCHAR(255) NOT NULL,
                    value       TEXT NOT NULL DEFAULT '',
                    description TEXT,
                    CONSTRAINT uq_system_config_key UNIQUE (key)
                );

                CREATE TABLE IF NOT EXISTS identity.system_error (
                    id            UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                    message       TEXT,
                    detail        TEXT,
                    source        VARCHAR(255),
                    user_id       VARCHAR(128),
                    merchant_name VARCHAR(255),
                    created_at    TIMESTAMPTZ DEFAULT now()
                );
            ");

            // Block 5: Seed hierarchical data (Optional/Independent)
            try {
                await dbContext.Database.ExecuteSqlRawAsync(@"
                    INSERT INTO identity.tenant_node (node_code, tier_level, name, slug, status_id)
                    VALUES ('L1_ZAP_FASHION', 1, 'ZAP Fashion Holding', 'zap-fashion', 50) ON CONFLICT (node_code) DO NOTHING;

                    INSERT INTO identity.tenant_node (node_code, parent_id, tier_level, name, slug, status_id)
                    SELECT 'L2_ZAP_PREMIUM', id, 2, 'ZAP Premium Brand', 'zap-premium', 50
                    FROM identity.tenant_node WHERE node_code = 'L1_ZAP_FASHION' ON CONFLICT (node_code) DO NOTHING;

                    INSERT INTO identity.tenant_node (node_code, parent_id, tier_level, name, slug, status_id)
                    SELECT 'L6_PREM_Q1', id, 6, 'Showroom Quận 1', 'showroom-q1', 50
                    FROM identity.tenant_node WHERE node_code = 'L2_ZAP_PREMIUM' ON CONFLICT (node_code) DO NOTHING;
                ");
                Console.WriteLine("✅ Database schema initialized.");
            } catch (Exception ex) {
                Console.Error.WriteLine($"⚠️ Seed Data Warning: {ex.Message}");
            }
        }
        catch (Exception dbEx)
        {
            Console.Error.WriteLine($"⚠️ DB Init Error: {dbEx.Message}");
        }
        
        // --- Added Diagnostic Check ---
        try {
            await DbDiagnostic.Run(builder.Configuration.GetConnectionString("PostgreSql") ?? "");
        } catch (Exception diagEx) {
            Console.Error.WriteLine($"⚠️ Diagnostic Error: {diagEx.Message}");
        }
        // --------------------------------
    }

    app.MapHealthChecks("/healthz");
    app.MapControllers();

    Console.WriteLine("✅ Login API is running.");
    Console.WriteLine("👉 Local URL: http://localhost:5001/swagger/index.html");
    app.Run();
} catch (Exception ex) {
    Console.Error.WriteLine("FATAL ERROR DURING STARTUP:");
    Console.Error.WriteLine(ex.ToString());
    throw;
}
