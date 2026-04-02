$connString = "Host=136.118.121.105;Port=5432;Username=postgres;Password=Pg@Secret2026!;Database=zap_ecosystem"
$tablesToCheck = @(
    "commerce.terminal_registry_hd",
    "commerce.terminal_peripheral",
    "commerce.kiosk_profile",
    "commerce.order_header",
    "people.attendance_log"
)

Write-Host "--- Database Table existence Check ---"
foreach ($t in $tablesToCheck) {
    $schema, $name = $t.Split('.')
    $query = "SELECT EXISTS (SELECT FROM information_schema.tables WHERE table_schema = '$schema' AND table_name = '$name');"
    # Note: I don't have a psql client verified yet, but I can try to use Npgsql via dotnet if I had a small project.
    # Alternatively, I can just use 'psql' if it's in the path.
}

Write-Host "Checking for psql..."
psql --version
if ($LASTEXITCODE -eq 0) {
    foreach ($t in $tablesToCheck) {
        $schema, $name = $t.Split('.')
        $val = psql -d "postgresql://postgres:Pg@Secret2026!@136.118.121.105:5432/zap_ecosystem" -t -c "SELECT EXISTS (SELECT FROM information_schema.tables WHERE table_schema = '$schema' AND table_name = '$name');"
        Write-Host "$t : $val"
    }
} else {
    Write-Host "psql not found. Trying to use a small C# script."
}
