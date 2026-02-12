@echo off
echo ========================================
echo Starting Zap Identity API
echo ========================================
echo.

cd /d "D:\PROJECTS\2026\AllCRMAll\Zap.Backend"

echo Cleaning previous build...
dotnet clean services\Zap.Identity.Api\Zap.Identity.Api.csproj

echo.
echo Building project...
dotnet build services\Zap.Identity.Api\Zap.Identity.Api.csproj

echo.
echo Starting API...
echo API will be available at: http://localhost:5271
echo.
dotnet run --project services\Zap.Identity.Api\Zap.Identity.Api.csproj

pause
