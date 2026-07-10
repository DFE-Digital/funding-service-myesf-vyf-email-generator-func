# Manage Your Education and Skills Funding User Interface

The Manage Your Education and Skills Funding (MYESF) email generator allows the following:

- Generate emails
- ??
## Provider

[The Department for Education](https://www.gov.uk/government/organisations/department-for-education)

## About this project

This project is an ASP.NET Core 8 web api utilising Azure App Service for deployment.

The web api runs on an Azure App service on Azure.

**Note:** The project is currently being updated to be containerised via Docker where the deployment method and target will change, this document will be updated when these changes have been finalised.

# Local Configuration Guide

In order to run the application locally a valid `local.settings.json` file will need to be created in the `Pds.VYF.EmailGenerator.FuncApp` projects Below, and included in the repo, there is `local.settings.example.json` which can be used as a base and populated with the required values, which can be retrieved from the Azure Portal.

## Local Settings (`local.settings.json`)

```json
{
  "IsEncrypted": false,
  "Values": {
    "APPINSIGHTS_INSTRUMENTATIONKEY": "",
    "ASPNETCORE_ENVIRONMENT": "Staging",
    "AzureStorageConfiguration:ChildAuditTableName": "",
    "AzureStorageConfiguration:ConnectionString": "",
    "AzureStorageConfiguration:ControlTableName": "",
    "AzureStorageConfiguration:MaxPerPage": "50",
    "AzureStorageConfiguration:NotifyServiceTemplateTable": "NotifyServiceTemplateDetails",
    "AzureStorageConfiguration:ParentAuditTableName": "",
    "AzureWebJobsStorage": "",
    "AzureWebJobsDashboard": "",
    "CosmosDBConfiguration:AccountEndpoint": "",
    "CosmosDBConfiguration:AccountKey": "",
    "CosmosDBConfiguration:AuditCollectionName": "audit",
    "CosmosDBConfiguration:Database": "",
    "CosmosDBConfiguration:FundingGroupCollectionName": "",
    "CosmosDBConfiguration:MaxItemCount": "100",
    "CosmosDBConfiguration:ProviderFundingCollectionName": "",
    "DfESignin:PublicApi:ClientID": "",
    "DfESignin:PublicApi:ClientSecret": "",
    "DfESignin:PublicApi:Url": "",
    "FUNCTIONS_EXTENSION_VERSION": "~4",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "Logging:ApplicationInsights:Loglevel:Default": "Information",
    "Logging:ApplicationInsights:Loglevel:Microsoft": "Error",
    "Logging:LogLevel:Default": "Information",
    "PdsApplicationInsights:Environment": "",
    "PdsApplicationInsights:InstrumentationKey": "",
    "ServiceBusClientConfiguration:QueueName": "",
    "ServiceBusClientConfiguration:ServiceBusConnection": "",
    "ServiceConfiguration:EmailPublishedBatchSize": "50",
    "ServiceConfiguration:EmailRunMode": "Test",
    "ServiceConfiguration:EmailTemplates:ChildNewFunding": "",
    "ServiceConfiguration:EmailTemplates:ChildUpdatedFunding": "",
    "ServiceConfiguration:EmailTemplates:ParentNewAndUpdatedFundings": "",
    "ServiceConfiguration:EmailTemplates:ParentNewFundings": "",
    "ServiceConfiguration:EmailTemplates:ParentUpdatedFundings": "",
    "ServiceConfiguration:FundingFilterVariationReasons": "",
    "ServiceConfiguration:InternalEmailAddresses": "",
    "ServiceConfiguration:NotifyApiKeySecretName": "",
    "ServiceConfiguration:ParentSearchBatchSize": "20",
    "ServiceConfiguration:RequestingService": "",
    "ServiceConfiguration:TestEmailAddresses": "",
    "ServiceConfiguration:UIBaseUri": "",
    "ServiceConfiguration:UIChildUrl": "",
    "ServiceConfiguration:UIParentUrl": "",
    "TimerInterval": "",
    "VYFUIApiConfiguration:ApiKey": "",
    "VYFUIApiConfiguration:BaseUri": "",
    "VYFUIApiConfiguration:EmailEnabledFundingStreamAndPeriodsEndpointUri": "",
    "VYFUIApiConfiguration:LatestFundingStreamPublishedDateEndpointUri": "",
    "WEBSITE_ENABLE_SYNC_UPDATE_SITE": "true",
    "WEBSITE_RUN_FROM_PACKAGE": "1"
  }
}
```

### Setting Details

- **`APPINSIGHTS_INSTRUMENTATIONKEY`**  
  Unique string key for app insights.
 
- **`ASPNETCORE_ENVIRONMENT`**  
  Target environment.

- **`AzureStorageConfiguration:ChildAuditTableName`**  
  Azure storage table name for childaudit.

- **`AzureStorageConfiguration:ConnectionString`**  
  Unique string key for azure storage connection.

- **`AzureStorageConfiguration:ControlTableName`**  
  Azure storage table name for control table.

- **`AzureStorageConfiguration:MaxPerPage`**  
  Maximum numeric value for azure storage.

- **`AzureStorageConfiguration:NotifyServiceTemplateTable`**  
  Azure storage table name for NotifyServiceTemplate table.

- **`AzureStorageConfiguration:ParentAuditTableName`**  
  Azure storage table name for ParentAudit table.

- **`AzureWebJobsStorage`**  
  Unique connection string key for azure web jobs storage.
  
- **`AzureWebJobsDashboard`**  
  Indicate which storage environment to use.

- **`CosmosDBConfiguration:AccountEndpoint`**  
  A unique link to cosmosdb account endpoint.

- **`CosmosDBConfiguration:AccountKey`**  
  The connection string value used for accessing the VYF cosmos db service.
  
- **`CosmosDBConfiguration:AuditCollectionName`**  
  The name of the cosmos db collection used for audit purposes.
  
- **`CosmosDBConfiguration:Database`**  
  The name of the cosmos database to connect to.
  
- **`CosmosDBConfiguration:FundingGroupCollectionName`**  
  The name of the cosmos db collection used for funding group collection data.
  
- **`CosmosDBConfiguration:MaxItemCount`**  
  MAximum amount of items to go through in database.
  
- **`CosmosDBConfiguration:ProviderFundingCollectionName`**  
  The name of the cosmos db collection used for provider funding data.
  
- **`DfeSignIn:OpenIDConnect:Authority`**  
  The authority URL for DfE sign in Open ID Connect service.
  
- **`DfeSignIn:PublicApi:Clientid`**  
  The application (client) ID for DfE sign in public api service.

- **`DfeSignIn:PublicApi:ClientSecret`**  
  The application (client) secret for DfE sign in public api service.

- **`DfeSignIn:PublicApi:url`**  
  The url used to access DfE sign in public api service.

- **`FUNCTIONS_EXTENSION_VERSION`**  
  Extentions version for functions.
  
- **`FUNCTIONS_WORKER_RUNTIME`**  
  functions worker runtime.

- **`Logging:ApplicationInsights:LogLevel:Default`**
  The default logging level for the service when logging to Application Insights; refer to the [Microsoft Documentation](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.loglevel?view=net-9.0-pp) for an explanation of the different levels.

- **`Logging:ApplicationInsights:LogLevel:Microsoft`**
  The default logging level for Microsoft specific information when logging to Application Insights; refer to the [Microsoft Documentation](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.loglevel?view=net-9.0-pp) for an explanation of the different levels.

- **`Logging:LogLevel:Default`**
  The default logging level for the service; refer to the [Microsoft Documentation](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.loglevel?view=net-9.0-pp) for an explanation of the different levels.

- **`PdsApplicationInsights:Environment`**  
  The environment which the app is running on for Application Insights for logging purposes.
  
- **`PdsApplicationInsights:InstrumentationKey`**  
  The key value for Application Insights resource for logging purposes.

- **`ServiceBusClientConfiguration:QueueName`**  
  Service bus queue name.
  
- **`ServiceBusClientConfiguration:ServiceBusConnection`**  
  Unique connection string for service bus connection.

- **`ServiceConfiguration:EmailPublishedBatchSize`**  
  Numeric value for email batch size.
  
- **`ServiceConfiguration:EmailRunMode`**  
  Mode under which to run (test/dev,etc).

- **`ServiceConfiguration:EmailTemplates:ChildNewFunding`**  
  Unique string for ChildNewFunding template.
  
- **`ServiceConfiguration:EmailTemplates:ChildUpdatedFunding`**  
  Unique string for ChildUpdatedFunding template.

- **`ServiceConfiguration:EmailTemplates:ParentNewAndUpdatedFundings`**  
  Unique string for ParentNewAndUpdatedFundings template.

- **`ServiceConfiguration:EmailTemplates:ParentNewFundings`**  
  Unique string for ParentNewFundings template.

- **`ServiceConfiguration:EmailTemplates:ParentUpdatedFundings`**  
  Unique string for ParentUpdatedFundings template.

- **`ServiceConfiguration:FundingFilterVariationReasons`**  
  List of strings of variation reasons seperated by a comma.

- **`ServiceConfiguration:InternalEmailAddresses`**  
  Internal user email address.

- **`ServiceConfiguration:NotifyApiKeySecretName`**  
  Secret name for NotifyApi.

- **`ServiceConfiguration:ParentSearchBatchSize`**  
  Numeric value of parent search batch.
  
- **`ServiceConfiguration:RequestingService`**  
  Velue for service request.
  
- **`ServiceConfiguration:TestEmailAddresses`**  
  Internal user email address.

- **`ServiceConfiguration:UIBaseUri`**  
  Unique UI link.
  
- **`ServiceConfiguration:UIChildUrl`**  
  Unique url path for child url.
  
- **`ServiceConfiguration:UIParentUrl`**  
  Unique url path for child url.
  
- **`TimerInterval`**  
  Allowed time intervals to use.
  
- **`VYFUIApiConfiguration:ApiKey`**  
  Unique microsoft keyvault value.

- **`VYFUIApiConfiguration:BaseUri`**  
  Unique url path base uri.
  
- **`VYFUIApiConfiguration:EmailEnabledFundingStreamAndPeriodsEndpointUri`**  
  Unique url path for EmailEnabledFundingStreamAndPeriodsEndpointUri.

- **`VYFUIApiConfiguration:LatestFundingStreamPublishedDateEndpointUri`**  
  Unique url path for LatestFundingStreamPublishedDateEndpointUri.
  
- **`WEBSITE_ENABLE_SYNC_UPDATE_SITE`**  
  Boolean value for enabling sync updates.

- **`WEBSITE_RUN_FROM_PACKAGE`**  
  Number of package to run.

## Test execution

### Pds.VYF.EmailGenerator.Services.Tests

All the tests can be found in Pds.VYF.EmailGenerator.Services.Tests. There are no local settings files required for tests.
