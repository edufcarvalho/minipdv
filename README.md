# MiniPDV

A basic PDV software made as a technical challenge at my company.

## Table of Contents

- [MiniPDV](#minipdv)
  - [Table of Contents](#table-of-contents)
  - [Requirements](#requirements)
  - [Setup](#setup)
    - [Install .NET 10 SDK](#install-net-10-sdk)
    - [Install SQL Server](#install-sql-server)
      - [A. Docker](#a-docker)
      - [B. Manual Database Installation](#b-manual-database-installation)
    - [Installation](#installation)
    - [Build and run](#build-and-run)
    - [Running the Windows Forms UI](#running-the-windows-forms-ui)
  - [IIS Deployment](#iis-deployment)
  - [Configuration](#configuration)
  - [API Endpoints](#api-endpoints)

## Requirements

- Windows 10 or later (or Windows Server 2019+)
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- SQL Server (any edition that accepts TCP/IP connections)

## Setup

### Install .NET 10 SDK
Download and install from [dotnet.microsoft.com](https://dotnet.microsoft.com/download/dotnet/10.0).

Verify the installation:

```powershell
dotnet --version
```
### Install SQL Server

#### A. Docker
If you have `Docker Desktop` installed on Windows and a proper `WSL2` installation you can go to `Settings > General > Use the WSL 2 based engine` (MSSQL recent versions only run on Linux-based containers)
```bash
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=MiniPDV@2026!" -p 1433:1433 --name MINIPDV -v minipdv-data:/var/opt/mssql/data --user root -d mcr.microsoft.com/mssql/server:2025-latest
```

#### B. Manual Database Installation

If you already have a **SQL Server Standard** (or higher) instance running on your machine or network:

1. Download the installer from [microsoft.com/sql-server](https://www.microsoft.com/sql-server/sql-server-downloads) and run it.
2. Select **New SQL Server stand-alone installation**.
3. Choose **Mixed Mode** authentication and set the `sa` password.
4. Open **SQL Server Configuration Manager**, go to `SQL Server Network Configuration → Protocols for <instance>`, and **Enable TCP/IP**.
5. Restart the SQL Server service.
6. Update your `.env` file (see below) with the server address, database name, user, and password.

> **Note:** If your SQL Server is on a remote machine, make sure port `1433` is open in the firewall.

### Installation
```powershell
git clone https://github.com/edufcarvalho/minipdv.git minipdv
cd minipdv
```

Edit `.env` to match your SQL Server instance:

```env
DB_SERVER=localhost\MINPDV
DB_NAME=master
DB_USER=sa
DB_PASSWORD=MiniPDV@2026!
```

### Build and run
```powershell
# Restore dependencies
dotnet restore

# Build the project
dotnet build

# Run in API mode
dotnet run -- --api
```
The API will start on the default Kestrel ports (typically `http://localhost:5000`).

### Running the Windows Forms UI
```powershell
dotnet run
```

This launches the WinForms desktop interface (Windows only).

## IIS Deployment

Deploy the API directly to IIS on your Windows machine.

1. **Install IIS** and the [.NET 10 Hosting Bundle](https://dotnet.microsoft.com/download/dotnet/10.0) (pick *ASP.NET Core Runtime → Hosting Bundle*).
2. **Publish** the project:

```powershell
dotnet publish -c Release -o ./publish
```

3. Copy `./publish` to your IIS site folder (e.g.`C:\inetpub\wwwroot\minipdv`).
4. Create an **Application Pool** set to **No Managed Code**, **Integrated** pipeline.
5. Create a site pointing to the publish folder and select the app pool from step 4.
6. Place this `web.config` in the publish folder:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <system.webServer>
    <handlers>
      <add name="aspNetCore" path="*" verb="*" modules="AspNetCoreModuleV2" resourceType="Unspecified" />
    </handlers>
    <aspNetCore processPath=".\minipdv.exe" arguments="--api" stdoutLogEnabled="true"
                stdoutLogFile=".\logs\stdout" hostingModel="inprocess">
      <environmentVariables>
        <environmentVariable name="ASPNETCORE_ENVIRONMENT" value="Development" />
        <environmentVariable name="DB_SERVER" value="localhost\SQLEXPRESS" />
        <environmentVariable name="DB_NAME" value="SQLMINIPDV" />
        <environmentVariable name="DB_USER" value="sa" />
        <environmentVariable name="DB_PASSWORD" value="YourPassword" />
      </environmentVariables>
    </aspNetCore>
  </system.webServer>
</configuration>
```

The app detects IIS via the `ASPNETCORE_IIS_PHYSICAL_PATH` environment variable and runs in API mode automatically.

## Configuration

All database connection settings are stored in `.env` at the project root.

| Variable | Description | Default |
|---|---|---|
| `DB_SERVER` | SQL Server instance address | `localhost\SQLEXPRESS` |
| `DB_NAME` | Database name | `SQLMINIPDV` |
| `DB_USER` | Database user | `sa` |
| `DB_PASSWORD` | Database password | *(empty)* |

The connection string always uses `TrustServerCertificate=True`.

## API Endpoints

| Method | Path | Description |
|---|---|---|
| `GET` | `/api/health` | Health check — returns status and timestamp |
