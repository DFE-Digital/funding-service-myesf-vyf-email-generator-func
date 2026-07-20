// <copyright file="CosmosContainerServices.cs" company="Department for Education - Skill Funding Services">
// Copyright (c) Department for Education - Skill Funding Services. All rights reserved.
// </copyright>

using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Pds.VYF.EmailGenerator.Services.Abstract.InfraServices;
using Pds.VYF.EmailGenerator.Services.Enumerations;
using Pds.VYF.EmailGenerator.Services.Models.ConfigurationSettings;
using Pds.VYF.EmailGenerator.Services.Models.Responses;
using System.Diagnostics;

namespace Pds.VYF.EmailGenerator.Services.Services.InfraServices
{
    /// <summary>
    /// The class for CosmosContainerServices.
    /// </summary>
    /// <seealso cref="Pds.VYF.EmailGenerator.Services.Abstract.InfraServices.ICosmosContainerServices" />
    public class CosmosContainerServices : ICosmosContainerServices
    {
        private readonly ILogger<CosmosContainerServices> logger;
        private readonly CosmosSettings cosmosSettings;

        private readonly CosmosClient cosmosClient;
        private readonly Database database;

        private readonly Dictionary<string, Container> containersDic = [];

        /// <summary>
        /// Initializes a new instance of the <see cref="CosmosContainerServices" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="cosmosSettings">The cosmos settings.</param>
        public CosmosContainerServices(
                                ILogger<CosmosContainerServices> logger,
                                CosmosSettings cosmosSettings)
        {
            this.logger = logger;
            this.cosmosSettings = cosmosSettings;

            this.cosmosClient = new CosmosClient(
                                        this.cosmosSettings.AccountEndpoint,
                                        this.cosmosSettings.AccountKey,
                                        new CosmosClientOptions()
                                        {
                                            AllowBulkExecution = true,
                                            ConnectionMode = this.cosmosSettings.ConnectionMode,
                                            MaxRetryAttemptsOnRateLimitedRequests = 30,
                                            MaxRetryWaitTimeOnRateLimitedRequests = TimeSpan.FromSeconds(30),
                                        });
            this.database = this.cosmosClient.GetDatabase(this.cosmosSettings.Database);
        }

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <typeparam name="T">Any type.</typeparam>
        /// <param name="cosmosContainerNameEnum">The cosmos container name enum.</param>
        /// <param name="sqlQuery">The SQL query.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A <see cref="Task" /> representing the asynchronous operation.
        /// </returns>
        public async Task<CosmosQueryResponse<T>> GetAsync<T>(CosmosContainerNameEnum cosmosContainerNameEnum, string sqlQuery, CancellationToken cancellationToken)
        {
            var container = this.GetContainer(cosmosContainerNameEnum);

            bool isThroughputIncreased = false;

            var throughputAtStart = await container.ReadThroughputAsync(cancellationToken);

            try
            {
                return await this.GetAsyncInternal<T>(container, sqlQuery, cancellationToken);
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            {
                if (throughputAtStart >= this.cosmosSettings.HighThroughput)
                {
                    this.logger.LogInformation($"Container: {container.Id} throughput is {throughputAtStart} RU/s more than or equal to Higher Throughput: [{this.cosmosSettings.HighThroughput} RU/s] still not sufficient.");
                    throw;
                }
                else
                {
                    await container.ReplaceThroughputAsync(this.cosmosSettings.HighThroughput, cancellationToken: cancellationToken);
                    isThroughputIncreased = true;
                    this.logger.LogInformation($"Container: {container.Id} throughput increased to {this.cosmosSettings.HighThroughput} RU/s");
                }

                return await this.GetAsyncInternal<T>(container, sqlQuery, cancellationToken);
            }
            finally
            {
                var currentThroughput = await container.ReadThroughputAsync(cancellationToken);

                if (isThroughputIncreased == true && currentThroughput > throughputAtStart)
                {
                    await container.ReplaceThroughputAsync(throughputAtStart ?? this.cosmosSettings.LowThroughput, cancellationToken: cancellationToken);
                    this.logger.LogInformation($"Container {container.Id} throughput decreased to {throughputAtStart ?? 400} RU/s");
                }
            }
        }

        private async Task<CosmosQueryResponse<T>> GetAsyncInternal<T>(Container container, string sqlQuery, CancellationToken cancellationToken)
        {
            var sw = Stopwatch.StartNew();

            CosmosQueryResponse<T> response = new CosmosQueryResponse<T>();

            var requestOptions = new QueryRequestOptions()
            {
                MaxItemCount = this.cosmosSettings.MaxItemCount,
                MaxBufferedItemCount = this.cosmosSettings.MaxItemCount,
                MaxConcurrency = 0,
            };

            using var feedIterator = container.GetItemQueryIterator<T>(new QueryDefinition(sqlQuery), requestOptions: requestOptions);

            int batch = 0;

            while (feedIterator.HasMoreResults)
            {
                batch++;
                var feedResponse = await feedIterator.ReadNextAsync(cancellationToken);
                response.RequestCharge += feedResponse.RequestCharge;

                foreach (var item in feedResponse)
                {
                    response.Results.Add(item);
                }

                // logger.LogInformation($"Batch - {batch}, Query result extraction status code: {feedResponse.StatusCode}, Request Charge: {feedResponse.RequestCharge}, Extracted Items Count: {feedResponse.Resource.Count()}");
            }

            response.TimeTaken = sw.Elapsed;

            return response;
        }

        /// <summary>
        /// Gets the container.
        /// </summary>
        /// <param name="cosmosContainerNameEnum">The cosmos container name enum.</param>
        /// <returns>The Container (Cosmos).</returns>
        private Container GetContainer(CosmosContainerNameEnum cosmosContainerNameEnum)
        {
            string containerName = cosmosContainerNameEnum switch
            {
                CosmosContainerNameEnum.Funding => this.cosmosSettings.FundingGroupCollectionName,
                CosmosContainerNameEnum.ProviderFunding => this.cosmosSettings.ProviderFundingCollectionName,
                CosmosContainerNameEnum.Audit => this.cosmosSettings.AuditCollectionName,
                _ => throw new ArgumentException(null, nameof(cosmosContainerNameEnum)),
            };

            if (this.containersDic.TryGetValue(containerName, out Container? value))
            {
                return value;
            }
            else
            {
                var container = this.database.GetContainer(containerName);
                this.containersDic.TryAdd(containerName, container);
                return container;
            }
        }
    }
}