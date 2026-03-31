# PendoGo CRM Deployment Information

## 1. Core Services & Host Information
- **Repository**: [https://github.com/zap-vn/developer-backend-api](https://github.com/zap-vn/developer-backend-api)
- **CRM Gateway Host**: `https://crm-gateway-v1-c7wqwyi1.uc.gateway.dev`
- **Identity API Backend**: `https://pendogo-identity-api-957587570857.asia-southeast1.run.app`

## 2. Recent Deployment (2026-03-31)
- **Status**: SUCCESS (Build v6)
- **Changes**: 
  - Migrated to PostgreSQL only (removed MongoDB dependencies).
  - Fixed Foreign Key violation on `locale_id` by auto-seeding `identity.locale` and `identity.language` tables on startup.
  - Added diagnostics for schema verification.

## 3. Maintenance Commands
- **Build & Deploy**: `gcloud builds submit --config cloudbuild.yaml .`
- **Check Logs**: `gcloud run services logs read pendogo-identity-api --region asia-southeast1`
- **Check Service**: `gcloud run services describe pendogo-identity-api --region asia-southeast1`
