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
podman compose down 2>$null; if (-not $?) { }
podman compose up -d $service

Write-Host "`n=== Setup complete ($Env) ==="
Write-Host "API available at: http://localhost:5000"
Write-Host "Health endpoint: http://localhost:5000/api/health"
Write-Host "Doc endpoint: http://localhost:5000/swagger"

Write-Host "`nCompose commands:"
Write-Host "  podman compose ps"
Write-Host "  podman compose logs -f $service"
Write-Host "  podman compose logs -f db"
Write-Host "  podman compose down"
