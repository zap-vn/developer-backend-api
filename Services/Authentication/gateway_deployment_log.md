# LOG: API GATEWAY TROUBLESHOOTING & DEPLOYMENT STATUS
Date: 2026-03-12

## 1. Issue Resolved: Gateway 404 Error
- **Symptom**: Calling `https://pendogo-crm-gateway-b9ei3tej.uc.gateway.dev/api/Auth/check-account` returned a 404 "The current request is not defined by this API" error.
- **Root Cause**: The physical API gateway (`pendogo-crm-gateway`) was still running an older configuration that lacked the newly added `/api/Auth/check-account` and `/api/Auth/social-login` endpoints. Requests were rejected at the gateway level before reaching the Identity API backend. Furthermore, the gateway ID was `pendogo-crm-api` string, not `pendogo-identity-api` as assumed initially.
- **Resolution**:
  1. Updated the `src/api-gateway.yaml` file to include the new endpoints.
  2. Authenticated `gcloud` with `linh.nguyen@pendogo.vn`.
  3. Created a new API config pointing to the correct Gateway API (`pendogo-crm-api`):
     ```powershell
     gcloud api-gateway api-configs create identity-v12 --api=pendogo-crm-api --openapi-spec=api-gateway.yaml --project=pendogo-v1-6317
     ```
  4. Updated the gateway `pendogo-crm-gateway` in region `us-central1` to use this new configuration:
     ```powershell
     gcloud api-gateway gateways update pendogo-crm-gateway --api=pendogo-crm-api --api-config=identity-v12 --location=us-central1 --project=pendogo-v1-6317
     ```
- **Result**: The 404 error from the Gateway has been fixed. The Gateway now successfully forwards these routes to the backend `pendogo-identity-api` Cloud Run service.

## 2. Current Blocker: Deploying C# Backend to Cloud Run
- **Symptom**: The new C# logic governing Social Logins and unified Account Checks exists locally but has **not** been successfully deployed to the Cloud Run service (`pendogo-identity-api`).
- **Root Cause**: 
  - The local workspace (`d:\PROJECTS\2026\3_2`) is **not a Git repository**. Automatic CI/CD pipeline triggers (Github Actions) are not executing.
  - Manual deployment attempts via `gcloud builds submit` under `linh.nguyen@pendogo.vn` failed due to missing Google Cloud IAM permissions:
    - `artifactregistry.repositories.uploadArtifacts` denied (Cannot push docker image).
    - `run.services.get` denied (Cannot modify the Cloud Run service).

## 3. Required Next Steps for the User
To finalize the deployment of the new Authentication code, please take ONE of the following actions:

### Option A: Deploy via GitHub (Recommended)
1. Copy the updated code from this workspace (`d:\PROJECTS\2026\3_2\src\Services\Authentication`) to your actual local Git repository directory for this project.
2. Commit the changes (`git commit -m "feat: Add unified account check and social login API"`).
3. Push to the `main` branch (`git push`).
4. GitHub Actions will automatically handle the build and deployment using the service account `github-deployer@pendogo-v1-6317.iam.gserviceaccount.com` (which has the correct permissions).

### Option B: Fix IAM Permissions for Manual GCloud Deployment
Have your Organization Admin grant the `pendogo-v1-6317@gmail.com` account the following IAM Roles in the GCP Console for project `pendogo-v1-6317`:
- **Artifact Registry Writer**
- **Cloud Run Admin**
Once granted, you can manually build and deploy the container again from your terminal.

---
*End of Log*


