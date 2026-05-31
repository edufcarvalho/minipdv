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
# Ignore errors from "down" — it's just cleanup, and podman's external compose provider warning
# triggers a NativeCommandError under $ErrorActionPreference "Stop"
try { podman compose down 2>&1 | Out-Null } catch { }
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
