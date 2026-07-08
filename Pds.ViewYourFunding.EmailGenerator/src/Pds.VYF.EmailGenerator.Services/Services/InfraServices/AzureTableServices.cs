// <copyright file="AzureTableServices.cs" company="Department for Education - Skill Funding Services">
// Copyright (c) Department for Education - Skill Funding Services. All rights reserved.
// </copyright>

using System.Linq.Expressions;
using Azure;
using Azure.Core;
using Azure.Data.Tables;
using Microsoft.Extensions.Logging;
using Pds.VYF.EmailGenerator.Services.Abstract.InfraServices;
using Pds.VYF.EmailGenerator.Services.Models.AzureTableModels;
using Pds.VYF.EmailGenerator.Services.Models.ConfigurationSettings;

namespace Pds.VYF.EmailGenerator.Services.Services.InfraServices
{
    /// <summary>
    /// Storage Table Services.
    /// </summary>
    public class AzureTableServices : IAzureTableServices
    {
        private readonly TableServiceClient tableServiceClient;
        private readonly ILogger<AzureTableServices> logger;
        private readonly AzureTableSettings azureTableSettings;

        private readonly Dictionary<string, TableClient> tableClientDic = [];

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureTableServices" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="azureTableSettings">The azure table settings.</param>
        public AzureTableServices(
                                ILogger<AzureTableServices> logger,
                                AzureTableSettings azureTableSettings)
        {
            this.logger = logger;
            this.azureTableSettings = azureTableSettings;

            this.tableServiceClient = new TableServiceClient(
                                    this.azureTableSettings.ConnectionString,
                                    new TableClientOptions()
                                    {
                                        Retry =
                                                {
                                                    Delay = TimeSpan.FromSeconds(2),
                                                    MaxRetries = 20,
                                                    Mode = RetryMode.Exponential,
                                                    MaxDelay = TimeSpan.FromSeconds(30),
                                                    NetworkTimeout = TimeSpan.FromSeconds(100),
                                                },
                                    });
        }

        /// <summary>
        /// Queries the specified table name.
        /// </summary>
        /// <typeparam name="T">Any class inherits from <see cref="ITableEntity">ITableEntity</see>.</typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="select">The select.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<List<T>> Query<T>(string tableName, Expression<Func<T, bool>> filter, IEnumerable<string>? select = null, CancellationToken cancellationToken = default)
            where T : BaseTableEntity
        {
            List<T> results = new();

            var tableClient = await this.GetTableClient(tableName);

            AsyncPageable<T> queryResultsMaxPerPage = tableClient.QueryAsync(filter, this.azureTableSettings.MaxPerPage, select, cancellationToken);

            await foreach (Page<T> page in queryResultsMaxPerPage.AsPages())
            {
                results.AddRange(page.Values);
            }

            return results;

            // string continuationToken = null;
            // bool moreResultsAvailable = true;
            // while (moreResultsAvailable)
            // {
            //    var pages = tableClient
            //        .QueryAsync(filter, this.azureTableSettings.MaxPerPage, select, cancellationToken)
            //        .AsPages(continuationToken, pageSizeHint: this.azureTableSettings.MaxPerPage);
            //    //.FirstOrDefault(); // Note: Since the pageSizeHint only limits the number of results in a single page, we explicitly only enumerate the first page.

            // var page = await pages.;

            // if (page == null)
            //        break;

            // // Get the continuation token from the page.
            //    // Note: This value can be stored so that the next page query can be executed later.
            //    continuationToken = page.ContinuationToken;

            // IReadOnlyList<TableEntity> pageResults = page.Values;
            //    moreResultsAvailable = pageResults.Any() && continuationToken != null;

            // // Print out the results for this page.
            //    foreach (TableEntity result in pageResults)
            //    {
            //        Console.WriteLine($"{result.PartitionKey}-{result.RowKey}");
            //    }
            // }
        }

        /// <summary>
        /// Upserts the entities asynchronous.
        /// </summary>
        /// <typeparam name="T">Any type which override <see cref="BaseTableEntity" />.</typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="entities">The entities.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// Completed task.
        /// </returns>
        public async Task<int> UpsertEntitiesAsync<T>(string tableName, IEnumerable<T> entities, CancellationToken cancellationToken = default)
            where T : ITableEntity
        {
            var tableClient = await this.GetTableClient(tableName);
            var entitiesChunks = entities.Chunk(this.azureTableSettings.MaxPerPage);

            int successfullyProcessedCount = 0;

            foreach (var entitiesChunk in entitiesChunks)
            {
                try
                {
                    List<TableTransactionAction> entitiesBatch = new List<TableTransactionAction>();
                    entitiesBatch.AddRange(entitiesChunk.Select(e => new TableTransactionAction(TableTransactionActionType.UpsertReplace, e)));
                    Azure.Response<IReadOnlyList<Response>> response = await tableClient.SubmitTransactionAsync(entitiesBatch).ConfigureAwait(false);

                    successfullyProcessedCount += response?.Value?.Where(a => !a.IsError)?.Count() ?? 0;

                    // ToDo: Handle the error/Exceptions
                }
                catch (Exception)
                {
                }

                // ToDo: Handle the error/Exceptions
            }

            return successfullyProcessedCount; // ToDo: Return actually inserted Rows
        }

        /// <summary>
        /// Upserts the entity asynchronous.
        /// </summary>
        /// <typeparam name="T">Any type which override <see cref="BaseTableEntity" />.</typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// Completed task.
        /// </returns>
        public async Task<int> UpsertEntityAsync<T>(string tableName, T entity, CancellationToken cancellationToken = default)
            where T : ITableEntity
        {
            try
            {
                var tableClient = await this.GetTableClient(tableName);
                var response = await tableClient.UpsertEntityAsync(entity, TableUpdateMode.Replace, cancellationToken);

                if (response.IsError)
                {
                    this.logger.LogError("Error while processing Upsert Operation with error message: " + response.ReasonPhrase);
                }

                return response.IsError ? 0 : 1;
            }
            catch (Exception ex)
            {
                this.logger.LogError("Error while processing Upsert Operation with error message: " + ex.Message);
                return 0;
            }
        }

        /// <summary>
        /// Gets the table client.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>The Table Client.</returns>
        private async Task<TableClient> GetTableClient(string tableName)
        {
            if (this.tableClientDic.TryGetValue(tableName, out TableClient? value))
            {
                return value;
            }
            else
            {
                var tableClient = this.tableServiceClient.GetTableClient(tableName);
                await tableClient.CreateIfNotExistsAsync();
                this.tableClientDic.TryAdd(tableName, tableClient);
                return tableClient;
            }
        }
    }
}
