targetScope = 'resourceGroup'

@description('Short environment name, such as dev, stg, or prod.')
param environmentName string = 'dev'

@description('Azure region for all regional resources.')
param location string = resourceGroup().location

@secure()
param sqlAdministratorPassword string

param sqlAdministratorLogin string = 'npadmin'
param appName string = 'nonprofit-fund'

var namePrefix = '${appName}-${environmentName}'

module observability 'modules/observability.bicep' = {
  name: 'observability'
  params: {
    location: location
    namePrefix: namePrefix
  }
}

module data 'modules/data.bicep' = {
  name: 'data'
  params: {
    location: location
    namePrefix: namePrefix
    sqlAdministratorLogin: sqlAdministratorLogin
    sqlAdministratorPassword: sqlAdministratorPassword
  }
}

module apps 'modules/app-hosting.bicep' = {
  name: 'app-hosting'
  params: {
    location: location
    namePrefix: namePrefix
    appInsightsConnectionString: observability.outputs.appInsightsConnectionString
    sqlConnectionString: data.outputs.sqlConnectionString
    storageConnectionString: data.outputs.storageConnectionString
  }
}

module edge 'modules/edge.bicep' = {
  name: 'edge'
  params: {
    namePrefix: namePrefix
    webHostName: apps.outputs.webHostName
  }
}

output webUrl string = 'https://${apps.outputs.webHostName}'
output apiUrl string = 'https://${apps.outputs.apiHostName}'
output frontDoorEndpoint string = edge.outputs.frontDoorEndpoint

