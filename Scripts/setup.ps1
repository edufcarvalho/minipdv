param(
    [ValidateSet("Development", "Production")]
    [string]$Env = "Production"
)

$ErrorActionPreference = "Stop"

$service = if ($Env -eq "Development") { "api-dev" } else { "api" }

Write-Host "=== Setting up MiniPDV ($Env) ==="

if ($Env -eq "Production") {
    Write-Host "`n=== Building Docker image ==="
    podman compose build api
}

Write-Host "`n=== Starting services with Podman Compose ==="
try { podman compose down 2>&1 | Out-Null } catch { }
podman compose up -d $service

Write-Host "`n=== Waiting for API to be healthy ==="
$healthUrl = "http://localhost:5000/api/health"
$maxRetries = 60
$retryInterval = 3
$count = 0
$healthy = $false

while ($count -lt $maxRetries -and -not $healthy) {
    $count++
    try {
        $response = Invoke-WebRequest -Uri $healthUrl -UseBasicParsing -TimeoutSec 5
        if ($response.StatusCode -eq 200) {
            $healthy = $true
            Write-Host "API is healthy!"
        }
    } catch {
        if ($count -ge $maxRetries) {
            Write-Host "Error: API not healthy after $maxRetries attempts"
            exit 1
        }
        Write-Host "Waiting for API... (attempt $count/$maxRetries)"
        Start-Sleep -Seconds $retryInterval
    }
}

Write-Host "`n=== Setup complete ($Env) ==="
Write-Host "API available at: http://localhost:5000"
Write-Host "Health endpoint: http://localhost:5000/api/health"
Write-Host "Doc endpoint: http://localhost:5000/swagger"

Write-Host "`nCompose commands:"
Write-Host "  podman compose ps"
Write-Host "  podman compose logs -f $service"
Write-Host "  podman compose logs -f db"
Write-Host "  podman compose down"
