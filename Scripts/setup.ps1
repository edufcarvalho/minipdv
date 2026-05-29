$ErrorActionPreference = "Stop"

Write-Host "=== Building MiniPDV for Windows ==="
dotnet build -c Release
Write-Host "Windows build OK."

Write-Host "`n=== Starting services with Podman Compose ==="
podman compose down 2>$null; if (-not $?) { }
podman compose up -d

Write-Host "`n=== Setup complete ==="
Write-Host "API available at: http://localhost:5000"
Write-Host "`nCompose commands:"
Write-Host "  podman compose ps"
Write-Host "  podman compose logs -f api"
Write-Host "  podman compose logs -f db"
Write-Host "  podman compose down"
