// <copyright file="CosmosContainerServicesMocks.cs" company="Department for Education - Skill Funding Services">
// Copyright (c) Department for Education - Skill Funding Services. All rights reserved.
// </copyright>

using Moq;
using Pds.VYF.EmailGenerator.Services.Abstract.InfraServices;
using Pds.VYF.EmailGenerator.Services.Enumerations;
using Pds.VYF.EmailGenerator.Services.Models.Responses;

namespace Pds.VYF.EmailGenerator.Services.Tests.Mocks.InfraServices
{
    /// <summary>
    /// The class for CosmosClientResolverMocks.
    /// </summary>
    public static class CosmosContainerServicesMocks
    {
        /// <summary>
        /// Setups the get asynchronous.
        /// </summary>
        /// <typeparam name="T">Any Type.</typeparam>
        /// <param name="mock">The mock.</param>
        /// <param name="cosmosContainerNameEnum">The cosmos container name enum.</param>
        /// <param name="query">The query.</param>
        /// <param name="results">The results.</param>
        /// <param name="timeTaken">The time taken.</param>
        /// <param name="requestCharge">The request charge.</param>
        /// <returns>
        /// The same mock object after Get Setup.
        /// </returns>
        public static Mock<ICosmosContainerServices> SetupGetAsync<T>(this Mock<ICosmosContainerServices> mock, CosmosContainerNameEnum cosmosContainerNameEnum, string query, List<T> results, TimeSpan? timeTaken = null, double? requestCharge = null)
        {
            var cosmosResponse = new CosmosQueryResponse<T>()
            {
                TimeTaken = timeTaken ?? new TimeSpan(0, 1, 10),
                RequestCharge = requestCharge ?? 10,
            };

            cosmosResponse.Results.AddRange(results);

            mock
                .Setup(a => a.GetAsync<T>(cosmosContainerNameEnum, query, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cosmosResponse);

            return mock;
        }
    }
}
