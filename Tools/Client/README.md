# Bruno API Client

Bruno collection for testing the MiniPDV API. Supports both Development and Production environments.

## Installation

### VSCode Extension
Install [Bruno](https://marketplace.visualstudio.com/items?itemName=bruno-api-client.bruno) from the VSCode Marketplace.

### Standalone
Download from [Bruno Download Page](https://www.usebruno.com/downloads).

## Usage

1. Open Bruno, click the **plus icon → Open Collection**, and point to the `Tools\Client` folder.
2. Select the environment (`Development` or `Production`) from the environment dropdown.
3. Make sure the API is running (see [README.md](../../README.md#quick-start-docker-compose)).
4. Run **Login** first to populate `{{token}}`, then use **Check** or **Logout**.

## Endpoints

| Request | File | Description |
|---|---|---|
| Health | `Health/Health.yml` | Ping the health endpoint |
| Login | `Auth/Login.yml` | Authenticate (auto-saves token) |
| Register | `Auth/Register.yml` | Create user |
| Check | `Auth/Check.yml` | Validate token |
| Logout | `Auth/Logout.yml` | Revoke session |

## Variables

| Variable | Default (Development) | Default (Production) |
|---|---|---|
| `baseUrl` | `http://localhost:5000` | `http://localhost:5000` |
| `login` | `admin` | `admin` |
| `senha` | `123456` | `123456` |
| `nome` | `Admin` | `Admin` |
| `token` | *(set by Login)* | *(set by Login)* |

The `Login` request includes a post-response script that saves the JWT to the `{{token}}` variable automatically.
