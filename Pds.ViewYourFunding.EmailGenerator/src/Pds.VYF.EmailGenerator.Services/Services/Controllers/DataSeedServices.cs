// <copyright file="DataSeedServices.cs" company="Department for Education - Skill Funding Services">
// Copyright (c) Department for Education - Skill Funding Services. All rights reserved.
// </copyright>

using Microsoft.Extensions.Logging;
using Pds.VYF.EmailGenerator.Services.Abstract.Controllers;
using Pds.VYF.EmailGenerator.Services.Abstract.InfraServices;
using Pds.VYF.EmailGenerator.Services.Helpers;
using Pds.VYF.EmailGenerator.Services.Models.AzureTableModels;
using Pds.VYF.EmailGenerator.Services.Models.ConfigurationSettings;

namespace Pds.VYF.EmailGenerator.Services.Services.Controllers
{
    /// <summary>
    /// The class for DataSeedServices.
    /// </summary>
    /// <seealso cref="Pds.VYF.EmailGenerator.Services.Abstract.Controllers.IDataSeedServices" />
    /// <remarks>
    /// Initializes a new instance of the <see cref="DataSeedServices" /> class.
    /// </remarks>
    /// <param name="logger">The logger.</param>
    /// <param name="azureTableSettings">The azure table settings.</param>
    /// <param name="appSettings">The application settings.</param>
    /// <param name="azureTableServices">The azure table services.</param>
    /// <exception cref="System.ArgumentNullException">Please provide non null params.</exception>
    public class DataSeedServices(
                            ILogger<DataSeedServices> logger,
                            AzureTableSettings azureTableSettings,
                            AppSettings appSettings,
                            IAzureTableServices azureTableServices) : IDataSeedServices
    {
        /// <summary>
        /// Seeds the asynchronous.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task SeedAsync()
        {
            logger.LogInformation("Seeding of initial value is started.");
            await this.SeedAzureStorageTableReferenceDataInternal();
            logger.LogInformation("Seeding of initial value is Completed.");
        }

        /// <summary>
        /// Seeds the azure storage table reference data internal.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">Please provide non null params.</exception>
        private async Task SeedAzureStorageTableReferenceDataInternal()
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(appSettings.RequestingService);
            ArgumentException.ThrowIfNullOrWhiteSpace(appSettings.NotifyApiKeySecretName);

            var allExistingEntities = await azureTableServices.Query<NotifyServiceTemplateDetails>(azureTableSettings.NotifyServiceTemplateTable, a => true);
            var lastUsedRowKey = allExistingEntities.Max(a => int.TryParse(a.RowKey, out int intRowKey) ? intRowKey : 0);
            int nextRowKey = lastUsedRowKey + 1;

            foreach (var partitionKey in appSettings.EmailTemplates.GetAllEmailMessageTypes())
            {
                var emailTemplateId = appSettings.EmailTemplates.GetTemplateId(partitionKey);

                if (emailTemplateId is null)
                {
                    logger.LogError($"Email Template Id for Email Message Type (partitionKey): {partitionKey} is not present in the Config. No action made. Please ensure the values are set correctly to update them in the table {azureTableSettings.NotifyServiceTemplateTable}.");
                }
                else
                {
                    var currentEntity = await this.GetCurrentNotifyServiceTemplateEntry(partitionKey);

                    if (currentEntity is null)
                    {
                        await this.InsertNotifyServiceTemplateEntry(nextRowKey, partitionKey, emailTemplateId);
                        nextRowKey++;
                    }
                    else if (this.HasUpdateRequired(emailTemplateId, currentEntity))
                    {
                        await this.UpdateNotifyServiceTemplateEntry(emailTemplateId, currentEntity);
                    }
                    else
                    {
                        logger.LogInformation($"No changes required in the {azureTableSettings.NotifyServiceTemplateTable} Entry for Partition Key: {currentEntity.PartitionKey} and RowKey: {currentEntity.RowKey}.");
                    }
                }
            }
        }

        private async Task<NotifyServiceTemplateDetails?> GetCurrentNotifyServiceTemplateEntry(string partitionKey)
        {
            var notifyServiceTemplateDetails = await azureTableServices
                                                    .Query<NotifyServiceTemplateDetails>(
                                                        azureTableSettings.NotifyServiceTemplateTable,
                                                        a => a.PartitionKey == partitionKey && a.RequestingService == appSettings.RequestingService);

            return notifyServiceTemplateDetails.FirstOrDefault();
        }

        private async Task InsertNotifyServiceTemplateEntry(int rowKey, string partitionKey, string emailTemplateId)
        {
            logger.LogInformation($"The New {azureTableSettings.NotifyServiceTemplateTable} Entry creation started for Partition Key: {partitionKey} and RowKey: {rowKey}.");
            var notifyServiceTemplateDetails = new NotifyServiceTemplateDetails(partitionKey, rowKey.ToString())
            {
                RequestingService = appSettings.RequestingService,
                TemplateId = emailTemplateId,
                NotifyApiKeySecretName = appSettings.NotifyApiKeySecretName,
            };
            await azureTableServices.UpsertEntityAsync(azureTableSettings.NotifyServiceTemplateTable, notifyServiceTemplateDetails);
            logger.LogInformation($"The new {azureTableSettings.NotifyServiceTemplateTable} Entry created successfully for Partition Key: {partitionKey} and RowKey: {rowKey}.");
        }

        private bool HasUpdateRequired(string emailTemplateId, NotifyServiceTemplateDetails currentEntity)
        {
            return currentEntity.NotifyApiKeySecretName != appSettings.NotifyApiKeySecretName
                                || currentEntity.RequestingService != appSettings.RequestingService
                                || currentEntity.TemplateId != emailTemplateId;
        }

        private async Task UpdateNotifyServiceTemplateEntry(string? emailTemplateId, NotifyServiceTemplateDetails currentEntity)
        {
            logger.LogInformation($"The Existing {azureTableSettings.NotifyServiceTemplateTable} Entry modification started for Partition Key: {currentEntity.PartitionKey} and RowKey: {currentEntity.RowKey}.");
            currentEntity.NotifyApiKeySecretName = appSettings.NotifyApiKeySecretName;
            currentEntity.TemplateId = emailTemplateId;
            currentEntity.Timestamp = DateTime.UtcNow;

            await azureTableServices.UpsertEntityAsync(azureTableSettings.NotifyServiceTemplateTable, currentEntity);
            logger.LogInformation($"The Existing {azureTableSettings.NotifyServiceTemplateTable} Entry modification completed for Partition Key: {currentEntity.PartitionKey} and RowKey: {currentEntity.RowKey}.");
        }
    }
}