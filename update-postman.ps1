# File: update-postman.ps1
$POSTMAN_API_KEY = "YOUR_POSTMAN_API_KEY_HERE" # Replace with your PMAK key locally. DO NOT COMMIT THE REAL KEY.
$ENV_ID = "4f87ab66-2f52-4d39-a90e-32e68194e33a"
$TOKEN = $(gcloud.cmd auth print-identity-token)

# Cấu trúc JSON tối giản và đầy đủ để tránh lỗi 500
$body = @{
    environment = @{
        name   = "[DEV] - CRM MERCHANT"
        values = @(
            @{
                key     = "google_token"
                value   = $TOKEN
                enabled = $true
            }
        )
    }
} | ConvertTo-Json -Depth 10 -Compress

try {
    Invoke-RestMethod -Method Put -Uri "https://api.getpostman.com/environments/$ENV_ID" `
        -Header @{ "X-Api-Key" = $POSTMAN_API_KEY } `
        -ContentType "application/json" `
        -Body $body
    Write-Host "✅ CẬP NHẬT THÀNH CÔNG! Hãy quay lại Postman nhấn Send." -ForegroundColor Green
}
catch {
    Write-Host "❌ LỖI RỒI: " -ForegroundColor Red
    $_.Exception.Message
}
