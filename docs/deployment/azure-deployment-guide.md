# Azure Deployment Guide

## Prerequisites

- Azure subscription
- Resource group
- GitHub Actions OIDC service principal
- Azure AD B2C tenant and app registration
- SQL admin password stored as `SQL_ADMIN_PASSWORD`

## Deploy Infrastructure

```powershell
az login
az group create --name rg-nonprofit-fund-dev --location eastus
az deployment group create `
  --resource-group rg-nonprofit-fund-dev `
  --template-file infra/bicep/main.bicep `
  --parameters environmentName=dev sqlAdministratorPassword='<password>'
```

## Configure GitHub

Add repository secrets:

- `AZURE_CLIENT_ID`
- `AZURE_TENANT_ID`
- `AZURE_SUBSCRIPTION_ID`
- `SQL_ADMIN_PASSWORD`

Add repository variable:

- `AZURE_RESOURCE_GROUP`

Run `deploy-dev` from GitHub Actions.

