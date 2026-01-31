# secrets-management

## Overview

This repository includes a reusable Key Vault secret provider library and a .NET 8 Web API that exposes secrets via HTTP.

## Configuration

Set the Key Vault URI in the Web API configuration:

- `KeyVault:VaultUri` - the URI of your Azure Key Vault, for example `https://your-vault-name.vault.azure.net/`.

You can configure this in `appsettings.Development.json` or via environment variables.

## Usage

Run the API and request a secret by name:

```
GET /secrets/{name}
```

The endpoint returns the secret value in plain text and requires authorization by default. Configure authentication/authorization and transport security before exposing it beyond trusted networks.

The endpoint uses `DefaultAzureCredential`, so ensure your environment is authenticated with Azure (for example, `az login` or managed identity).
