# Manage Your Education and Skills Funding View Your Funding Email Generator

The Manage Your Education and Skills Funding (MYESF) View Your Funding (VYF) Email Generator is a service that generates emails to notify providers of new or updated funding information. The service will:
- Generate email notification messages for providers when new funding is published or when existing funding is updated.
- Which will then be sent to the [MYESF Shared Email Processor](https://github.com/DFE-Digital/funding-service-myesf-shared-email-processor-func) for processing via GOV.UK Notify.

## Provider

[The Department for Education](https://www.gov.uk/government/organisations/department-for-education)

## About this project

This project is a .Net 8 Isolated Worker Azure Function project utilizing an Azure Function App for deployment.

**Note:** The project is currently being updated to be containerised via Docker where the deployment method and target will change, this document will be updated when these changes have been finalised.

# Local Configuration Guide

In order to run the application locally a valid `local.settings.json` file will need to be created in the `Pds.VYF.EmailGenerator.FuncApp` projects Below, and included in the repo, there is `local.settings.example.json` which can be used as a base and populated with the required values, which can be retrieved from the Azure Portal.

## Local Settings (`local.settings.json`)

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureStorageConfiguration:ChildAuditTableName": "",
    "AzureStorageConfiguration:ConnectionString": "",
    "AzureStorageConfiguration:ControlTableName": "",
    "AzureStorageConfiguration:MaxPerPage": "50",
    "AzureStorageConfiguration:NotifyServiceTemplateTable": "",
    "AzureStorageConfiguration:ParentAuditTableName": "",
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "AzureWebJobsDashboard": "UseDevelopmentStorage=true",
    "CosmosDBConfiguration:AccountEndpoint": "",
    "CosmosDBConfiguration:AccountKey": "",
    "CosmosDBConfiguration:AuditCollectionName": "",
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
    "VYFUIApiConfiguration:LatestFundingStreamPublishedDateEndpointUri": ""
  }
}
```

### Setting Details

- **`AzureStorageConfiguration:ChildAuditTableName`**  
  The table name of the Azure storage child audit table.

- **`AzureStorageConfiguration:ConnectionString`**  
  The connection string of the Azure storage account resource.

- **`AzureStorageConfiguration:ControlTableName`**  
  The table name of the Azure storage email generator control table.

- **`AzureStorageConfiguration:MaxPerPage`**  
  The value of the maximum number of results contained in a page.

- **`AzureStorageConfiguration:NotifyServiceTemplateTable`**  
  The table name of the Azure storage notify service template details table.

- **`AzureStorageConfiguration:ParentAuditTableName`**  
  The table name of the Azure storage parent audit table.

- **`AzureWebJobsStorage`**  
  The Azure Storage connection string required by the Azure Functions runtime for operation and trigger management.
  
- **`AzureWebJobsDashboard`**  
  The Azure Storage jobs dashboard configuration setting to resolve issues with local running.

- **`CosmosDBConfiguration:AccountEndpoint`**  
  The url of the Cosmos Db resource.

- **`CosmosDBConfiguration:AccountKey`**  
  The unique connection key of the Cosmos Db resource.
  
- **`CosmosDBConfiguration:AuditCollectionName`**  
  The name of the Cosmos Db collection used for audit purposes.
  
- **`CosmosDBConfiguration:Database`**  
  The name of the Cosmos Db database.
  
- **`CosmosDBConfiguration:FundingGroupCollectionName`**  
  The name of the Cosmos Db collection used for funding data.
  
- **`CosmosDBConfiguration:MaxItemCount`**  
  The value of the maximum amount of records to be returned from the Cosmos query.
  
- **`CosmosDBConfiguration:ProviderFundingCollectionName`**  
  The name of the Cosmos Db collection used for provider funding data.
  
- **`DfeSignIn:OpenIDConnect:Authority`**  
  The authority URL for DfE sign in Open ID Connect service.
  
- **`DfeSignIn:PublicApi:Clientid`**  
  The application (client) ID for DfE sign in public api service.

- **`DfeSignIn:PublicApi:ClientSecret`**  
  The application (client) secret for DfE sign in public api service.

- **`DfeSignIn:PublicApi:url`**  
  The url used to access DfE sign in public api service.

- **`FUNCTIONS_EXTENSION_VERSION`**  
  The Azure Functions runtime version used by the application.
  
- **`FUNCTIONS_WORKER_RUNTIME`**  
  The worker runtime used by the Function App. This application uses the .NET Isolated worker model.

- **`Logging:ApplicationInsights:LogLevel:Default`**
  The default logging level for the service when logging to Application Insights; refer to the [Microsoft Documentation](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.loglevel?view=net-9.0-pp) for an explanation of the different levels.

- **`Logging:ApplicationInsights:LogLevel:Microsoft`**
  The default logging level for Microsoft specific information when logging to Application Insights; refer to the [Microsoft Documentation](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.loglevel?view=net-9.0-pp) for an explanation of the different levels.

- **`Logging:LogLevel:Default`**
  The default logging level for the service; refer to the [Microsoft Documentation](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.loglevel?view=net-9.0-pp) for an explanation of the different levels.

- **`PdsApplicationInsights:Environment`**  
  The environment which the app is running on for Application Insights for logging purposes.
  
- **`PdsApplicationInsights:InstrumentationKey`**  
  The key for Application Insights resource for logging purposes.

- **`ServiceBusClientConfiguration:QueueName`**  
  The name of the service bus queue to send the email notification message to.
  
- **`ServiceBusClientConfiguration:ServiceBusConnection`**  
  The connection string of the Service Bus resource.

- **`ServiceConfiguration:EmailPublishedBatchSize`**  
  The value of the number of emails which will be generated within one batch during the generation process.
  
- **`ServiceConfiguration:EmailRunMode`**  
  The conditional value for how the generation process should handle emails recipients based on environment.

- **`ServiceConfiguration:EmailTemplates:ChildNewFunding`**  
  The template id for the ChildNewFunding email template.
  
- **`ServiceConfiguration:EmailTemplates:ChildUpdatedFunding`**  
  The template id for the ChildUpdatedFunding email template.

- **`ServiceConfiguration:EmailTemplates:ParentNewAndUpdatedFundings`**  
  The template id for the ParentNewAndUpdatedFundings email template.

- **`ServiceConfiguration:EmailTemplates:ParentNewFundings`**  
  The template id for the ParentNewFundings email template.

- **`ServiceConfiguration:EmailTemplates:ParentUpdatedFundings`**  
  The template id for the ParentUpdatedFundings email template.

- **`ServiceConfiguration:FundingFilterVariationReasons`**  
  The comma seperated list value of the funding variation reasons which depict whether an email should be generated.

- **`ServiceConfiguration:InternalEmailAddresses`**  
  The email address used for internal email notifications.

- **`ServiceConfiguration:NotifyApiKeySecretName`**  
  The value of the notify api key paramter used in the email notification message.

- **`ServiceConfiguration:ParentSearchBatchSize`**  
  The value of the number of parent providers which will be searched within one batch during the generation process.
  
- **`ServiceConfiguration:RequestingService`**  
  The value of the requesting service parameter used in the email notification message.
  
- **`ServiceConfiguration:TestEmailAddresses`**  
  The email address used when the function app is configured to run in `Test` mode.

- **`ServiceConfiguration:UIBaseUri`**  
  The value of the MYESF UI url parameter used in the email notification message.
  
- **`ServiceConfiguration:UIChildUrl`**  
  The value of the MYESF UI child provider view url parameter used in the email notification message.
  
- **`ServiceConfiguration:UIParentUrl`**  
  The value of the MYESF UI parent provider view url parameter used in the email notification message.
  
- **`TimerInterval`**  
  The CRON expression defining the schedule used by the timer-triggered email generation process.
  
- **`VYFUIApiConfiguration:ApiKey`**  
  The api secret key of View Your Funding external api.

- **`VYFUIApiConfiguration:BaseUri`**  
  The url of View Your Funding external api.
  
- **`VYFUIApiConfiguration:EmailEnabledFundingStreamAndPeriodsEndpointUri`**  
  The url of View Your Funding external api email enabled funding stream endpoint.

- **`VYFUIApiConfiguration:LatestFundingStreamPublishedDateEndpointUri`**  
  The url of View Your Funding external api latest funding stream published date endpoint.

## Build and Test

To build and test locally, you can either use Visual Studio, Visual Studio Code or simply use dotnet CLI `dotnet build` and `dotnet test` more information in dotnet CLI can be found at <https://docs.microsoft.com/en-us/dotnet/core/tools/>.

## Contribute

To contribute,

- If you are part of the team then create a branch for changes and then submit your changes for review by creating a pull request.
- If you are external to the organisation then fork this repository and make necessary changes and then submit your changes for review by creating a pull request.
