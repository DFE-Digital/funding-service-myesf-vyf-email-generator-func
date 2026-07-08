// <copyright file="CosmosQueryServicesMocks.cs" company="Department for Education - Skill Funding Services">
// Copyright (c) Department for Education - Skill Funding Services. All rights reserved.
// </copyright>

using Moq;
using Pds.VYF.EmailGenerator.Services.Abstract.InfraServices;
using Pds.VYF.EmailGenerator.Services.Models.Requests;

namespace Pds.VYF.EmailGenerator.Services.Tests.Mocks.InfraServices
{
    /// <summary>
    /// The Extension class which provides mocks for CosmosQueryServices.
    /// </summary>
    public static class CosmosQueryServicesMocks
    {
        /// <summary>
        /// Gets the mock with default implementation.
        /// </summary>
        /// <returns>Mocks.</returns>
        public static Mock<ICosmosQueryServices> GetMockWithDefaultImplementation()
        {
            var mockCosmosQueryServices = new Mock<ICosmosQueryServices>(MockBehavior.Strict);

            mockCosmosQueryServices
                .Setup(a => a.GetParentQuery(It.IsAny<ProcessRequest>(), It.IsAny<string>()))
                .Returns(It.IsAny<string>())
                .Verifiable();

            mockCosmosQueryServices
                .Setup(a => a.GetChildQuery(It.IsAny<ProcessRequest>(), It.IsAny<string>()))
                .Returns(It.IsAny<string>())
                .Verifiable();

            mockCosmosQueryServices
                .Setup(a => a.GetChildWithParentIdQuery(It.IsAny<ProcessRequest>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>()))
                .Returns(It.IsAny<string>())
                .Verifiable();

            mockCosmosQueryServices
                .Setup(a => a.GetLastFeedReaderAuditQuery())
                .Returns(It.IsAny<string>())
                .Verifiable();

            return mockCosmosQueryServices;
        }

        /// <summary>
        /// Setups the get child with parent identifier query.
        /// </summary>
        /// <param name="mock">The mock.</param>
        /// <param name="ids">The ids.</param>
        /// <param name="query">The query.</param>
        /// <returns>The Mock.</returns>
        public static Mock<ICosmosQueryServices> SetupGetChildWithParentIdQuery(this Mock<ICosmosQueryServices> mock, IEnumerable<string> ids, string query)
        {
            mock
                .Setup(a => a.GetChildWithParentIdQuery(It.IsAny<ProcessRequest>(), It.IsAny<string>(), ids))
                .Returns(query)
                .Verifiable();

            return mock;
        }
    }
}
