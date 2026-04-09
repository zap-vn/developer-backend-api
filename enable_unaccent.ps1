Add-Type -Path "d:\PROJECTS\4_2026\01042026\Services\Product\CRM.Product.Infrastructure\bin\Debug\net9.0\Npgsql.dll"
$conn = [Npgsql.NpgsqlConnection]::new("Host=136.118.121.105;Port=5432;Username=postgres;Password=Pg@Secret2026!;Database=zap_ecosystem")
$conn.Open()
$cmd = $conn.CreateCommand()
$cmd.CommandText = "CREATE EXTENSION IF NOT EXISTS unaccent;"
$cmd.ExecuteNonQuery()
$conn.Close()
Write-Host "unaccent extension created successfully!"
