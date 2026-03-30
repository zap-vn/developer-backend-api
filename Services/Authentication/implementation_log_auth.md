# AUTHENTICATION REFACTORING & SOCIAL LOGIN IMPLEMENTATION LOG
Date: 2026-03-12

## 1. Objective
Refactor the authentication flow to support a unified account check (Email/Phone), implement OTP verification for registration, and add support for Social Login (Google, Facebook, Apple).

## 2. Changes Implemented

### 2.1. Unified Account Availability Check
- **API**: `POST /api/Auth/check-account`
- **Command**: `CheckAccountAvailabilityCommand`
- **Logic**: 
    - Accepts a single `Email` field (can contain email or phone string).
    - Checks for existence in both `Email` and `Phone` columns in the `User` table.
    - If `Provider` is "Email", generates and sends a 6-digit OTP via `IEmailService`.
    - If `Provider` is a Social one (Google, Facebook, Apple), it skips OTP and returns success immediately.
- **Errors**: Standardized Vietnamese messages with keys (e.g., `error_duplicate_account|Email này đã được sử dụng...`).

### 2.2. OTP Verification
- **API**: `POST /api/Auth/verify-registration-otp`
- **Command**: `VerifyRegistrationOtpCommand`
- **Logic**: Validates the latest OTP code against the identifier, checking for expiry and usage.

### 2.3. Social Login Support
- **API**: `POST /api/Auth/social-login`
- **Command**: `SocialAuthCommand`
- **Logic**: 
    - Unified handler for Google, Facebook, and Apple.
    - If user doesn't exist, it automatically registers them as a MerchantAdmin with `IsVerify = true`.
    - Synchronizes new users to the **Customer Service** in the background.
    - Generates and returns a JWT token for immediate login.

## 3. Infrastructure Updates

### 3.1. API Gateway (`src/api-gateway.yaml`)
- Added new paths and CORS support (`OPTIONS` handlers) for:
    - `/api/Auth/check-account`
    - `/api/Auth/verify-registration-otp`
    - `/api/Auth/social-login`
- Target backend: `pendogo-identity-api` on Cloud Run.

### 3.2. Automated Deployment (`.github/workflows/cloudbuild.yaml`)
- Configured Cloud Build to build the container from the root level and deploy to Cloud Run with necessary environment variables (MongoDB connection, JWT Secret).

## 4. Current Status & Next Steps
- **Code Status**: Fully implemented and tested locally.
- **Deployment**: Blocked by expired GCloud credentials in this terminal.
- **Action Required by User**:
    1. Run `cmd /c "gcloud auth login"`.
    2. Run Gateway update commands to apply the new `api-gateway.yaml` (fixes 404 errors).
    3. Run `cmd /c "gcloud builds submit --config=../.github/workflows/cloudbuild.yaml ."` in `src` folder.

## 5. Skill Documentation
Created a new skill definition at:
`d:\PROJECTS\2026\3_2\.agent\skills\crm-pre-registration-check\SKILL.md`

