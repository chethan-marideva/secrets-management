# secrets-management

## Overview

This repository includes a reusable Key Vault secret provider library and a .NET 8 Web API that exposes secrets via HTTP.

## Configuration

Set the Key Vault URI in the Web API configuration:

- `KeyVault:VaultUri` - the URI of your Azure Key Vault, for example `https://your-vault-name.vault.azure.net/`.
- `Authentication:ApiKey` - API key required for requests (sent as `X-Api-Key`).

You can configure this in `appsettings.Development.json` or via environment variables.

## Usage

Run the API and request a secret by name:

```
GET /secrets/{name}
X-Api-Key: <api-key>
```

The endpoint returns the secret value in plain text and requires an API key. Use it only in trusted environments; keep it behind TLS and restricted network access, or consider exposing short-lived tokens instead of raw secrets.

The endpoint uses `DefaultAzureCredential`, so ensure your environment is authenticated with Azure (for example, `az login` or managed identity).
