@echo off
echo ================================================
echo Starting Zap Identity API
echo ================================================
echo.

cd services\Zap.Identity.Api

echo Cleaning previous builds...
dotnet clean >nul 2>&1

echo Building project...
dotnet build

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ================================================
    echo Build successful! Starting API...
    echo The API will be available at: http://localhost:5271
    echo Press Ctrl+C to stop the API
    echo ================================================
    echo.
    dotnet run
) else (
    echo.
    echo ================================================
    echo Build failed! Please check the errors above.
    echo ================================================
    pause
)
