using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Pds.Core.DfESignIn.Interfaces;
using Pds.Core.DfESignIn.Models;
using Pds.Core.Notification.Interfaces;
using Pds.Core.Notification.Models;
using Pds.Core.Utils.Interfaces;
using Pds.VYF.EmailGenerator.Services.Abstract.Controllers;
using Pds.VYF.EmailGenerator.Services.Enumerations;
using Pds.VYF.EmailGenerator.Services.Models.AzureTableModels;
using Pds.VYF.EmailGenerator.Services.Models.ConfigurationSettings;
using Pds.VYF.EmailGenerator.Services.Models.Requests;
using Pds.VYF.EmailGenerator.Services.Tests.Mocks.Loggers;


namespace Pds.VYF.EmailGenerator.Services.Services.Processors.Tests
{
    /// <summary>
    /// The test class for EmailPublisher.
    /// </summary>
    [TestClass]
    public class EmailPublisherTests
    {
        private readonly MockLogger<EmailPublisher> mockLogger = new();
        private readonly Mock<IAuditAndControlServices> mockAuditAndControlServices = new(MockBehavior.Strict);
        private readonly Mock<AppSettings> mockAppSettings = new(MockBehavior.Strict);
        private readonly Mock<INotificationEmailQueueService> mockNotificationEmailQueueService = new(MockBehavior.Strict);
        private readonly Mock<IRetryMechanism> mockRetryMechanism = new(MockBehavior.Strict);
        private readonly Mock<IDfESignInPublicApi> mockDfESignInPublicApi = new(MockBehavior.Strict);

        private readonly EmailPublisher emailPublisher;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailPublisherTests"/> class.
        /// </summary>
        public EmailPublisherTests()
        {
            this.emailPublisher = new(
                                    this.mockLogger.Object,
                                    this.mockAuditAndControlServices.Object,
                                    this.mockAppSettings.Object,
                                    this.mockNotificationEmailQueueService.Object,
                                    this.mockRetryMechanism.Object,
                                    this.mockDfESignInPublicApi.Object);
        }

        /// <summary>
        /// Publishes the asynchronous test test email addresses missing.
        /// </summary>
        [TestMethod]
        public void PublishAsyncTest_TestEmailAddressesMissing()
        {
            // Arrange
            this.mockAppSettings.SetupGet(a => a.EmailRunMode).Returns(EmailRunModeEnum.Test);
            this.mockAppSettings.SetupGet(a => a.TestEmailAddresses).Returns(It.IsIn(null, string.Empty));

            // Act
            Action action = () =>
            {
                this.emailPublisher.PublishAsync<ParentAuditTableModel>(
                                        new ProcessRequest(),
                                        [],
                                        a => string.Empty,
                                        a => new Dictionary<string, object?>(),
                                        string.Empty,
                                        CancellationToken.None).Wait();

            };

            // Assert
            action.Should().Throw<ArgumentException>().WithMessage($"During Test Email Run Mode, the TestEmailAddresses should not be null or empty.");
        }

        /// <summary>
        /// Publishes the asynchronous test test email addresses missing.
        /// </summary>
        [TestMethod]
        public void PublishAsyncTest_InternalEmailAddressesMissing()
        {
            // Arrange
            this.mockAppSettings.SetupGet(a => a.EmailRunMode).Returns(EmailRunModeEnum.Internal);
            this.mockAppSettings.SetupGet(a => a.InternalEmailAddresses).Returns(It.IsIn(null, string.Empty));

            // Act
            Action action = () =>
            {
                this.emailPublisher.PublishAsync<ParentAuditTableModel>(
                                        new ProcessRequest(),
                                        [],
                                        a => string.Empty,
                                        a => new Dictionary<string, object?>(),
                                        string.Empty,
                                        CancellationToken.None).Wait();

            };

            // Assert
            action.Should().Throw<ArgumentException>().WithMessage($"During Internal Email Run Mode, the InternalEmailAddresses should not be null or empty.");
        }

        /// <summary>
        /// Publishes the asynchronous test test email addresses missing.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task PublishAsyncTest_ValidValue()
        {
            // Arrange
            this.mockAppSettings.SetupGet(a => a.EmailRunMode).Returns(EmailRunModeEnum.Internal);
            this.mockAppSettings.SetupGet(a => a.InternalEmailAddresses).Returns("1@1.com");
            this.mockAppSettings.SetupGet(a => a.RequestingService).Returns(It.IsAny<string>());
            this.mockAppSettings.SetupGet(a => a.EmailPublishedBatchSize).Returns(1);

            this.mockDfESignInPublicApi
                .Setup(a => a.GetUserContactsForOrganisation(It.IsAny<int>(), "ViewAllocationStatements"))
                .ReturnsAsync(new UserContactLookupResponse() { Users = [new() { Email = "1@1.com" }] });

            this.mockNotificationEmailQueueService.Setup(a => a.SendAsync(It.IsAny<NotificationMessage>())).Returns(Task.CompletedTask).Verifiable(Times.Exactly(3));

            this.mockAuditAndControlServices.Setup(a => a.UpsertAuditEntryAsync(It.IsAny<ProcessRequest>(), It.IsAny<string>(), It.IsAny<ParentAuditTableModel>(), It.IsAny<CancellationToken>())).ReturnsAsync(It.IsAny<int>()).Verifiable(Times.Exactly(3));

            List<ParentAuditTableModel> auditEntries = [
                new() { EmailPublishStatus = EmailPublishStatusEnum.InitialEntry, PartitionKey = "PK1", RowKey = "1", UKPRN = "10001000" },
                new() { EmailPublishStatus = EmailPublishStatusEnum.InitialEntry, PartitionKey = "PK2", RowKey = "2", UKPRN = "10001001" },
                new() { EmailPublishStatus = EmailPublishStatusEnum.InitialEntry, PartitionKey = "PK3", RowKey = "3", UKPRN = "10001002" },
                ];

            // Act
            var actualValue = await this.emailPublisher.PublishAsync(
                                        new ProcessRequest(),
                                        auditEntries,
                                        a => It.IsAny<string>(),
                                        a => It.IsAny<Dictionary<string, object?>>(),
                                        string.Empty,
                                        new CancellationToken(false));

            // Assert
            actualValue.Should().Be(3);

            this.mockLogger.VerifyLog(LogLevel.Information, "Publishing email of batch", Times.Exactly(6));

            this.mockNotificationEmailQueueService.VerifyAll();
            this.mockAuditAndControlServices.VerifyAll();

            auditEntries.Should().OnlyContain(a => a.EmailPublishStatus == EmailPublishStatusEnum.EmailPublished);
        }

        /// <summary>
        /// Gets the email addresses test invalid ukprn.
        /// </summary>
        /// <param name="ukprn">The ukprn.</param>
        [TestMethod]
        [DataRow("123")]
        [DataRow("Abc")]
        [DataRow("ABCDEFGH")]
        [DataRow("123456789")]
        [DataRow("")]
        [DataRow(null)]
        public void GetEmailAddressesTest_InvalidUKPRN(string ukprn)
        {
            // Act
            this.emailPublisher.GetEmailAddresses(ukprn).Wait();

            // Assert
            this.mockLogger.VerifyLog(LogLevel.Error, " provided. It should be 8 digit whole number", Times.Once());
        }

        /// <summary>
        /// Gets the email addresses test success.
        /// </summary>
        /// <param name="ukprn">The ukprn.</param>
        /// <param name="emailIds">The email ids.</param>
        /// <returns>
        /// A <see cref="Task" /> representing the asynchronous operation.
        /// </returns>
        [TestMethod]
        [DataRow(12345678, "1@1.com", "1@2.com")]
        [DataRow(12345679)]
        public async Task GetEmailAddressesTest_Success(int ukprn, params string[] emailIds)
        {
            // Arrange
            var userContacts = emailIds.Select(a => new UserContact { Email = a });

            this.mockDfESignInPublicApi
                .Setup(a => a.GetUserContactsForOrganisation(ukprn, "ViewAllocationStatements"))
                .ReturnsAsync(new UserContactLookupResponse() { Ukprn = ukprn.ToString(), Users = userContacts });

            // Act
            var actualResult = await this.emailPublisher.GetEmailAddresses(ukprn.ToString());

            // Assert
            actualResult.Should().Equal(emailIds);
        }
    }
}