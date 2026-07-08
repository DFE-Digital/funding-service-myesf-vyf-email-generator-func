// <copyright file="ParentProcessorTests.cs" company="Department for Education - Skill Funding Services">
// Copyright (c) Department for Education - Skill Funding Services. All rights reserved.
// </copyright>

using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Pds.VYF.EmailGenerator.Services.Abstract.ApiConnectors;
using Pds.VYF.EmailGenerator.Services.Abstract.Controllers;
using Pds.VYF.EmailGenerator.Services.Abstract.InfraServices;
using Pds.VYF.EmailGenerator.Services.Abstract.Processors;
using Pds.VYF.EmailGenerator.Services.Enumerations;
using Pds.VYF.EmailGenerator.Services.Models.AzureTableModels;
using Pds.VYF.EmailGenerator.Services.Models.CosmosModels;
using Pds.VYF.EmailGenerator.Services.Models.Requests;
using Pds.VYF.EmailGenerator.Services.Tests.Mocks.InfraServices;
using Pds.VYF.EmailGenerator.Services.Tests.Mocks.Loggers;

namespace Pds.VYF.EmailGenerator.Services.Services.Processors.Tests
{
    /// <summary>
    /// The Test class for ParentProcessor.
    /// </summary>
    [TestClass]
    public class ParentProcessorTests
    {
        private readonly MockLogger<ParentProcessor> mockLogger = new();
        private readonly Mock<ICosmosContainerServices> cosmosContainerServicesMock = new(MockBehavior.Strict);
        private readonly Mock<IAuditAndControlServices> mockAuditAndControlServices = new(MockBehavior.Strict);
        private readonly MockCosmosQueryServices mockCosmosQueryServices = new();
        private readonly Mock<IEmailPublisher> mockEmailPublisher = new(MockBehavior.Strict);
        private readonly Mock<IVYFUIServices> mockVYFUIServices = new();

        private readonly ParentProcessor parentProcessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParentProcessorTests"/> class.
        /// </summary>
        public ParentProcessorTests()
        {
            this.parentProcessor = new(
                                this.mockLogger.Object,
                                this.cosmosContainerServicesMock.Object,
                                this.mockAuditAndControlServices.Object,
                                new(),
                                new() { ParentSearchBatchSize = 2, UIBaseUri = "https://example.com", UIParentUrl = "test" },
                                this.mockCosmosQueryServices.Object,
                                this.mockEmailPublisher.Object,
                                this.mockVYFUIServices.Object);
        }

        /// <summary>
        /// Extracts the test.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task ExtractTest()
        {
            // Arrange
            ProcessRequest processRequest = new();

            List<ParentCosmosModel> parentCosmosModels = [new() { Id = "P1" }, new() { Id = "P2" }, new() { Id = "P3" }, new() { Id = "P4" }, new() { Id = "P5" }];

            this.cosmosContainerServicesMock
                .SetupGetAsync(CosmosContainerNameEnum.Funding, It.IsAny<string>(), parentCosmosModels);

            this
                .mockCosmosQueryServices
                .SetupGetChildWithParentIdQuery(["P1", "P2"], "batch1Query")
                .SetupGetChildWithParentIdQuery(["P3", "P4"], "batch2Query")
                .SetupGetChildWithParentIdQuery(["P5"], "batch3Query");

            static ChildWithParentIdCosmosModel CWPBuilder(string parentId, string id, TypeOfFundingEnum typeOfFunding, string providerName) => new()
            {
                ParentId = parentId,
                Id = id,
                TypeOfFunding = typeOfFunding,
                ProviderName = providerName,
            };

            this.cosmosContainerServicesMock
                .SetupGetAsync(
                        CosmosContainerNameEnum.ProviderFunding,
                        "batch1Query",
                        [
                            CWPBuilder("P1", "C1", TypeOfFundingEnum.New, "N1"),
                            CWPBuilder("P1", "C2", TypeOfFundingEnum.Updated, "N2"),
                            CWPBuilder("P1", "C3", TypeOfFundingEnum.New, "N3"),
                            CWPBuilder("P2", "C4", TypeOfFundingEnum.Updated, "N4"),
                            CWPBuilder("P2", "C5", TypeOfFundingEnum.Updated, "N5"),
                        ])
                .SetupGetAsync(
                        CosmosContainerNameEnum.ProviderFunding,
                        "batch2Query",
                        [
                            CWPBuilder("P3", "C6", TypeOfFundingEnum.New, "N6"),
                            CWPBuilder("P4", "C7", TypeOfFundingEnum.New, "N7"),
                            CWPBuilder("P4", "C8", TypeOfFundingEnum.New, "N8"),
                            CWPBuilder("P4", "C9", TypeOfFundingEnum.New, "N9"),
                            CWPBuilder("P4", "C10", TypeOfFundingEnum.New, "N10"),
                        ])
                .SetupGetAsync(
                        CosmosContainerNameEnum.ProviderFunding,
                        "batch3Query",
                        [
                            CWPBuilder("P5", "C11", TypeOfFundingEnum.New, "N11"),
                            CWPBuilder("P5", "C12", TypeOfFundingEnum.Updated, "N12"),
                            CWPBuilder("P5", "C13", TypeOfFundingEnum.New, "N13"),
                            CWPBuilder("P5", "C14", TypeOfFundingEnum.New, "N14"),
                            CWPBuilder("P5", "C15", TypeOfFundingEnum.Updated, "N15"),
                        ]);

            static ParentCosmosModel PBuilder(string id, string providersWithNewFunding, string providersWithUpdatedFunding, int newProviderFundingCount, int updatedProviderFundingCount) => new()
            {
                Id = id,
                ProvidersWithNewFunding = providersWithNewFunding,
                ProvidersWithUpdatedFunding = providersWithUpdatedFunding,
                NewProviderFundingCount = newProviderFundingCount,
                UpdatedProviderFundingCount = updatedProviderFundingCount,
            };

            List<ParentCosmosModel> expectedResult = [
                PBuilder("P1", "1. N1\n2. N3", "1. N2", 2, 1),
                PBuilder("P2", string.Empty, "1. N4\n2. N5", 0, 2),
                PBuilder("P3", "1. N6", string.Empty, 1, 0),
                PBuilder("P4", "1. N10\n2. N7\n3. N8\n4. N9", string.Empty, 4, 0),
                PBuilder("P5", "1. N11\n2. N13\n3. N14", "1. N12\n2. N15", 3, 2),
              ];

            // Act
            var actualResult = await this.parentProcessor.Extract(processRequest, "2024-10-10", new CancellationToken(false));

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
            this.mockLogger
                .VerifyLog(LogLevel.Information, "Extraction of Parent fundings from Cosmos is", Times.Exactly(2))
                .VerifyLog(LogLevel.Information, "Extraction of Child Providers under the Parent organization from Cosmos is", Times.Exactly(2));

        }

        /// <summary>
        /// Transforms the test.
        /// </summary>
        [TestMethod]
        public void TransformTest()
        {
            // Arrange
            List<ParentCosmosModel> parentCosmosModels = [
                    new() { Id = "Id1", NewProviderFundingCount = 1, UpdatedProviderFundingCount = 0, },
                    new() { Id = "Id2", NewProviderFundingCount = 23, UpdatedProviderFundingCount = 0, },
                    new() { Id = "Id3", NewProviderFundingCount = 0, UpdatedProviderFundingCount = 1, },
                    new() { Id = "Id4", NewProviderFundingCount = 0, UpdatedProviderFundingCount = 15, },
                    new() { Id = "Id5", NewProviderFundingCount = 0, UpdatedProviderFundingCount = 0, },
                ];

            // Act
            var actualResult = this.parentProcessor.Transform(new ProcessRequest(), parentCosmosModels);

            // Assert
            actualResult.Should().NotBeNull();
            actualResult.Should().HaveCount(parentCosmosModels.Count);
            actualResult.Where(a => a.EmailPublishStatus == EmailPublishStatusEnum.InitialEntry).Should().HaveCount(4);
            actualResult.Where(a => a.EmailPublishStatus == EmailPublishStatusEnum.EmailSkipped).Should().HaveCount(1);

            foreach (var auditModel in actualResult)
            {
                if (auditModel.NewProviderFundingCount > 0 || auditModel.UpdatedProviderFundingCount > 0)
                {
                    auditModel.EmailPublishStatus.Should().Be(EmailPublishStatusEnum.InitialEntry);
                }
                else
                {
                    auditModel.EmailPublishStatus.Should().Be(EmailPublishStatusEnum.EmailSkipped);
                }
            }

            this.mockLogger.VerifyLog(LogLevel.Information, "Transforming Cosmos to Audit data is Successfully completed!", Times.Once());
        }

        /// <summary>
        /// Gets the message type test.
        /// </summary>
        /// <param name="newCount">The new count.</param>
        /// <param name="updateCount">The update count.</param>
        /// <param name="expectedResult">The expected result.</param>
        [TestMethod]
        [DataRow(15, 0, "ParentNewFundings")]
        [DataRow(0, 1, "ParentUpdatedFundings")]
        [DataRow(0, 15, "ParentUpdatedFundings")]
        [DataRow(1, 1, "ParentNewAndUpdatedFundings")]
        [DataRow(15, 1, "ParentNewAndUpdatedFundings")]
        [DataRow(1, 15, "ParentNewAndUpdatedFundings")]
        [DataRow(15, 15, "ParentNewAndUpdatedFundings")]
        public void GetMessageTypeTest(int newCount, int updateCount, string expectedResult)
        {
            // Arrange
            ParentAuditTableModel parentAuditTableModel = new ParentAuditTableModel()
            {
                NewProviderFundingCount = newCount,
                UpdatedProviderFundingCount = updateCount,
            };

            // Act
            var actualResult = this.parentProcessor.GetMessageType(parentAuditTableModel);

            // Assert
            actualResult.Should().Be(expectedResult);
        }

        /// <summary>
        /// Gets the personalisation test.
        /// </summary>
        [TestMethod]
        public void GetPersonalisationTest_NewAndUpdatedBothHaveMultipleChildern()
        {
            // Arrange
            var auditEntity = new ParentAuditTableModel
            {
                FundingStreamCode = "GAG",
                OrganizationName = "Sample Parent",
                NewProviderFundingCount = 2,
                UpdatedProviderFundingCount = 3,
                ProvidersWithNewFunding = "ProviderA, ProviderB",
                ProvidersWithUpdatedFunding = "ProviderX, ProviderY",
            };

            // Act
            var result = this.parentProcessor.GetPersonalisation(auditEntity);

            // Assert
            result.Should().NotBeNull();
            result.Should().ContainKey("fundingStream").And.ContainValue("General Annual Grant");
            result.Should().ContainKey("ParentName").And.ContainValue("Sample Parent");
            result.Should().ContainKey("totalNewandUpdatedStatementCount").And.ContainValue(5);
            result.Should().ContainKey("linktoallocationstatementspage").And.ContainValue("https://example.com/test");

            // Check personalisations related to new statements
            result.Should().ContainKey("newStatementCount").And.ContainValue(2);
            result.Should().ContainKey("totalNewStatementCount").And.ContainValue(2);
            result.Should().ContainKey("NStatements").And.ContainValue("statements");
            result.Should().ContainKey("NProviderNameList").And.ContainValue("ProviderA, ProviderB");

            // Check personalisations related to updated statements
            result.Should().ContainKey("totalUpdatedStatementCount").And.ContainValue(3);
            result.Should().ContainKey("updatedStatementCount").And.ContainValue(3);
            result.Should().ContainKey("UStatements").And.ContainValue("statements");
            result.Should().ContainKey("UProviderNameList").And.ContainValue("ProviderX, ProviderY");
        }

        /// <summary>
        /// Gets the personalisation test.
        /// </summary>
        [TestMethod]
        public void GetPersonalisationTest_NewAndUpdatedBothHaveOneChild()
        {
            // Arrange
            var auditEntity = new ParentAuditTableModel
            {
                FundingStreamCode = "GAG",
                OrganizationName = "Sample Parent",
                NewProviderFundingCount = 1,
                UpdatedProviderFundingCount = 1,
                ProvidersWithNewFunding = "ProviderA",
                ProvidersWithUpdatedFunding = "ProviderX",
            };

            // Act
            var result = this.parentProcessor.GetPersonalisation(auditEntity);

            // Assert
            result.Should().NotBeNull();
            result.Should().ContainKey("fundingStream").And.ContainValue("General Annual Grant");
            result.Should().ContainKey("ParentName").And.ContainValue("Sample Parent");
            result.Should().ContainKey("totalNewandUpdatedStatementCount").And.ContainValue(2);
            result.Should().ContainKey("linktoallocationstatementspage").And.ContainValue("https://example.com/test");

            // Check personalisations related to new statements
            result.Should().ContainKey("newStatementCount").And.ContainValue(1);
            result.Should().ContainKey("totalNewStatementCount").And.ContainValue(1);
            result.Should().ContainKey("NStatements").And.ContainValue("statement");
            result.Should().ContainKey("NProviderNameList").And.ContainValue("ProviderA");

            // Check personalisations related to updated statements
            result.Should().ContainKey("totalUpdatedStatementCount").And.ContainValue(1);
            result.Should().ContainKey("updatedStatementCount").And.ContainValue(1);
            result.Should().ContainKey("UStatements").And.ContainValue("statement");
            result.Should().ContainKey("UProviderNameList").And.ContainValue("ProviderX");
        }

        /// <summary>
        /// Gets the personalisation test.
        /// </summary>
        [TestMethod]
        public void GetPersonalisationTest_Newonly()
        {
            // Arrange
            var auditEntity = new ParentAuditTableModel
            {
                FundingStreamCode = "GAG",
                OrganizationName = "Sample Parent",
                NewProviderFundingCount = 2,
                UpdatedProviderFundingCount = 0,
                ProvidersWithNewFunding = "ProviderA, ProviderB",
            };

            // Act
            var result = this.parentProcessor.GetPersonalisation(auditEntity);

            // Assert
            result.Should().NotBeNull();
            result.Should().ContainKey("fundingStream").And.ContainValue("General Annual Grant");
            result.Should().ContainKey("ParentName").And.ContainValue("Sample Parent");
            result.Should().ContainKey("totalNewandUpdatedStatementCount").And.ContainValue(2);
            result.Should().ContainKey("linktoallocationstatementspage").And.ContainValue("https://example.com/test");

            // Check personalisations related to new statements
            result.Should().ContainKey("newStatementCount").And.ContainValue(2);
            result.Should().ContainKey("totalNewStatementCount").And.ContainValue(2);
            result.Should().ContainKey("NStatements").And.ContainValue("statements");
            result.Should().ContainKey("NProviderNameList").And.ContainValue("ProviderA, ProviderB");

            // Check personalisations related to updated statements
            result.Should().NotContainKey("totalUpdatedStatementCount");
            result.Should().NotContainKey("updatedStatementCount");
            result.Should().NotContainKey("UStatements");
            result.Should().NotContainKey("UProviderNameList");
        }

        /// <summary>
        /// Gets the personalisation test.
        /// </summary>
        [TestMethod]
        public void GetPersonalisationTest_Updateonly()
        {
            // Arrange
            var auditEntity = new ParentAuditTableModel
            {
                FundingStreamCode = "GAG",
                OrganizationName = "Sample Parent",
                NewProviderFundingCount = 0,
                UpdatedProviderFundingCount = 2,
                ProvidersWithUpdatedFunding = "ProviderX, ProviderY",
            };

            // Act
            var result = this.parentProcessor.GetPersonalisation(auditEntity);

            // Assert
            result.Should().NotBeNull();
            result.Should().ContainKey("fundingStream").And.ContainValue("General Annual Grant");
            result.Should().ContainKey("ParentName").And.ContainValue("Sample Parent");
            result.Should().ContainKey("totalNewandUpdatedStatementCount").And.ContainValue(2);
            result.Should().ContainKey("linktoallocationstatementspage").And.ContainValue("https://example.com/test");

            // Check personalisations related to new statements
            result.Should().NotContainKey("newStatementCount");
            result.Should().NotContainKey("totalNewStatementCount");
            result.Should().NotContainKey("NStatements");
            result.Should().NotContainKey("NProviderNameList");

            // Check personalisations related to updated statements
            result.Should().ContainKey("totalUpdatedStatementCount").And.ContainValue(2);
            result.Should().ContainKey("updatedStatementCount").And.ContainValue(2);
            result.Should().ContainKey("UStatements").And.ContainValue("statements");
            result.Should().ContainKey("UProviderNameList").And.ContainValue("ProviderX, ProviderY");
        }
    }
}