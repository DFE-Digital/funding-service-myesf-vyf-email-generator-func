using Moq;
using Pds.VYF.EmailGenerator.Services.Abstract.InfraServices;
using Pds.VYF.EmailGenerator.Services.Enumerations;
using Pds.VYF.EmailGenerator.Services.Models.Responses;

namespace Pds.VYF.EmailGenerator.Services.Tests.Mocks.InfraServices
{
    /// <summary>
    /// The Mock Cosmos Container Services.
    /// </summary>
    /// <seealso cref="Moq.Mock&lt;Pds.VYF.EmailGenerator.Services.Abstract.InfraServices.ICosmosContainerServices&gt;" />
    public class MockCosmosContainerServices() : Mock<ICosmosContainerServices>(MockBehavior.Strict)
    {
        /// <summary>
        /// Setups the get asynchronous.
        /// </summary>
        /// <typeparam name="T">Any type.</typeparam>
        /// <param name="cosmosContainerNameEnum">The cosmos container name enum.</param>
        /// <param name="query">The query.</param>
        /// <param name="results">The results.</param>
        /// <param name="timeTaken">The time taken.</param>
        /// <param name="requestCharge">The request charge.</param>
        /// <returns>The same object for chaining.</returns>
        public MockCosmosContainerServices SetupGetAsync<T>(
                                            CosmosContainerNameEnum cosmosContainerNameEnum,
                                            string query,
                                            List<T> results,
                                            TimeSpan? timeTaken = null,
                                            double? requestCharge = null)
        {
            var cosmosResponse = new CosmosQueryResponse<T>()
            {
                TimeTaken = timeTaken ?? new(0, 1, 10),
                RequestCharge = requestCharge ?? 10,
            };

            cosmosResponse.Results.AddRange(results);

            this
                .Setup(a => a.GetAsync<T>(cosmosContainerNameEnum, query, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cosmosResponse);

            return this;
        }
    }
}
