// <copyright file="EmailPublisher.cs" company="Department for Education - Skill Funding Services">
// Copyright (c) Department for Education - Skill Funding Services. All rights reserved.
// </copyright>

using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Pds.Core.DfESignIn.Interfaces;
using Pds.Core.Notification.Interfaces;
using Pds.Core.Notification.Models;
using Pds.Core.Utils.Interfaces;
using Pds.VYF.EmailGenerator.Services.Abstract.Controllers;
using Pds.VYF.EmailGenerator.Services.Abstract.Processors;
using Pds.VYF.EmailGenerator.Services.Enumerations;
using Pds.VYF.EmailGenerator.Services.Extensions;
using Pds.VYF.EmailGenerator.Services.Models.AzureTableModels;
using Pds.VYF.EmailGenerator.Services.Models.ConfigurationSettings;
using Pds.VYF.EmailGenerator.Services.Models.Requests;

namespace Pds.VYF.EmailGenerator.Services.Services.Processors
{
    /// <summary>
    /// The class for EmailPublisher.
    /// </summary>
    /// <seealso cref="Pds.VYF.EmailGenerator.Services.Abstract.Processors.IEmailPublisher" />
    /// <remarks>
    /// Initializes a new instance of the <see cref="EmailPublisher" /> class.
    /// </remarks>
    /// <param name="logger">The logger.</param>
    /// <param name="auditAndControlServices">The audit and control services.</param>
    /// <param name="appSettings">The application settings.</param>
    /// <param name="notificationEmailQueueService">The notification email queue service.</param>
    /// <param name="retryMechanism">The retry mechanism.</param>
    /// <param name="dfESignInPublicApi">The dfe sign in public API.</param>
    /// <exception cref="System.ArgumentNullException">All params should not be null.</exception>
    public class EmailPublisher(
                            ILogger<EmailPublisher> logger,
                            IAuditAndControlServices auditAndControlServices,
                            AppSettings appSettings,
                            INotificationEmailQueueService notificationEmailQueueService,
                            IRetryMechanism retryMechanism,
                            IDfESignInPublicApi dfESignInPublicApi)
        : IEmailPublisher
    {
        /// <summary>
        /// Publishes the asynchronous.
        /// </summary>
        /// <typeparam name="TAuditTableModel">Any class which inherits from BaseAuditTableModel.</typeparam>
        /// <param name="processRequest">The process request.</param>
        /// <param name="auditEntities">The audit entities.</param>
        /// <param name="messageTypeFunc">The message type function.</param>
        /// <param name="personalisationFunc">The personalisation function.</param>
        /// <param name="auditTableName">Name of the audit table.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <exception cref="System.ArgumentException">During Test Email Run Mode, the {nameof(this.appSettings.TestEmailAddresses)} should not be null or empty.</exception>
        public async Task<int> PublishAsync<TAuditTableModel>(
                                                   ProcessRequest processRequest,
                                                   IEnumerable<TAuditTableModel> auditEntities,
                                                   Func<TAuditTableModel, string> messageTypeFunc,
                                                   Func<TAuditTableModel, IDictionary<string, object?>> personalisationFunc,
                                                   string auditTableName,
                                                   CancellationToken cancellationToken)
            where TAuditTableModel : BaseAuditTableModel
        {
            if (appSettings.EmailRunMode == EmailRunModeEnum.Test && string.IsNullOrWhiteSpace(appSettings.TestEmailAddresses))
            {
                throw new ArgumentException($"During Test Email Run Mode, the {nameof(appSettings.TestEmailAddresses)} should not be null or empty.");
            }

            if (appSettings.EmailRunMode == EmailRunModeEnum.Internal && string.IsNullOrWhiteSpace(appSettings.InternalEmailAddresses))
            {
                throw new ArgumentException($"During Internal Email Run Mode, the {nameof(appSettings.InternalEmailAddresses)} should not be null or empty.");
            }

            ConcurrentBag<int> publishedCount = [];
            var auditEntitiesChunks = auditEntities.Chunk(appSettings.EmailPublishedBatchSize);
            int batchNo = 0;

            foreach (var auditEntitiesChunk in auditEntitiesChunks)
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    batchNo++;
                    logger.LogInformation($"Publishing email of batch {batchNo} is started. Batch size {auditEntitiesChunk.Length}");
                    List<Task> tasks = new(auditEntitiesChunk.Length * 2);
                    foreach (var auditEntity in auditEntitiesChunk)
                    {
                        if (!cancellationToken.IsCancellationRequested)
                        {
                            var task = this.PublishSingleEmailInternal(processRequest, auditEntity, messageTypeFunc, personalisationFunc, auditTableName, cancellationToken);
                            var resultTask = task.ContinueWith(a => publishedCount.Add(a.Result), cancellationToken);

                            tasks.Add(task);
                            tasks.Add(resultTask);
                        }
                    }

                    await Task.WhenAll(tasks);
                    logger.LogInformation($"Publishing email of batch {batchNo} is completed. Batch size {auditEntitiesChunk.Length}");
                }
            }

            return publishedCount.Sum();
        }

        /// <summary>
        /// Gets the email addresses.
        /// </summary>
        /// <param name="ukprn">The ukprn.</param>
        /// <returns>
        /// A <see cref="Task" /> representing the asynchronous operation.
        /// </returns>
        /// <exception cref="System.ArgumentException">The UKPRN should be 8 digit whole number.</exception>
        public async Task<IList<string>> GetEmailAddresses(string ukprn)
        {
            if (int.TryParse(ukprn, out var intUkprn) && ukprn?.Length == 8)
            {
                try
                {
                    var users = await dfESignInPublicApi.GetUserContactsForOrganisation(intUkprn, "ViewAllocationStatements");

                    return users?.Users?.Select(a => a.Email).ToList() ?? [];
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Error while getting email id for the UKPRN: {ukprn}");
                    return [];
                }
            }
            else
            {
                logger.LogError($"Invalid UKPRN ({ukprn}) provided. It should be 8 digit whole number");
                return [];
            }
        }

        /// <summary>
        /// Publishes the single email internal.
        /// </summary>
        /// <typeparam name="TAuditTableModel">The type of the audit table model.</typeparam>
        /// <param name="processRequest">The process request.</param>
        /// <param name="auditEntity">The audit entity.</param>
        /// <param name="messageTypeFunc">The message type function.</param>
        /// <param name="personalisationFunc">The personalisation function.</param>
        /// <param name="auditTableName">Name of the audit table.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        private async Task<int> PublishSingleEmailInternal<TAuditTableModel>(
            ProcessRequest processRequest,
            TAuditTableModel auditEntity,
            Func<TAuditTableModel, string> messageTypeFunc,
            Func<TAuditTableModel, IDictionary<string, object?>> personalisationFunc,
            string auditTableName,
            CancellationToken cancellationToken)
            where TAuditTableModel : BaseAuditTableModel
        {
            int result = 0;

            IList<string> emailAddressesFromDSI = await this.GetEmailAddresses(auditEntity.UKPRN) ?? [];

            IList<string> emailAddresses = appSettings.EmailRunMode switch
            {
                EmailRunModeEnum.Test => [.. appSettings.TestEmailAddresses?.Split(',')],
                EmailRunModeEnum.Internal => this.FilterOnlyInternalEmail(emailAddressesFromDSI),
                EmailRunModeEnum.Live => emailAddressesFromDSI,
                _ => [],
            };

            if (emailAddresses.Count > 0)
            {
                var messageType = messageTypeFunc(auditEntity);
                var personalisation = personalisationFunc(auditEntity);
                var message = this.BuildNotifyMessage(emailAddresses, messageType, personalisation);

                auditEntity.EmailAddresses = string.Join(",", emailAddressesFromDSI);

                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    await notificationEmailQueueService.SendAsync(message);
                    auditEntity.EmailPublishStatus = EmailPublishStatusEnum.EmailPublished;
                    auditEntity.EmailPublishedAt = DateTime.UtcNow.ToString();
                    result = 1;
                }
                catch (Exception ex)
                {
                    auditEntity.EmailPublishErrorMessage = ex.Message;
                    auditEntity.EmailPublishStatus = EmailPublishStatusEnum.EmailFailedToPublish;
                    logger.LogError(processRequest, "Email Publisher", ex, "Email failed to send.");
                }
            }
            else if (emailAddressesFromDSI.Count > 0 && appSettings.EmailRunMode == EmailRunModeEnum.Internal)
            {
                auditEntity.EmailPublishErrorMessage = "The email addresses found in the DFE Sign-In Public Api is not part of Internal Email Id";
                auditEntity.EmailAddresses = string.Join(",", emailAddressesFromDSI);
                auditEntity.EmailPublishStatus = EmailPublishStatusEnum.EmailFailedToPublish;
            }
            else
            {
                auditEntity.EmailPublishErrorMessage = "No email ID found in the DFE Sign-In Public Api";
                auditEntity.EmailPublishStatus = EmailPublishStatusEnum.EmailFailedToPublish;
            }

            await auditAndControlServices.UpsertAuditEntryAsync(processRequest, auditTableName, auditEntity, cancellationToken);
            return result;
        }

        /// <summary>
        /// Filters the only internal email.
        /// </summary>
        /// <param name="emailAddressesFromDSI">The email addresses from dsi.</param>
        /// <returns>List of filtered Email Address.</returns>
        private List<string> FilterOnlyInternalEmail(IList<string> emailAddressesFromDSI)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(appSettings.InternalEmailAddresses);

            return appSettings
                    .InternalEmailAddresses
                    .Split(",")
                    .Where(internalId => emailAddressesFromDSI.Any(id => id.Equals(internalId, StringComparison.OrdinalIgnoreCase)))
                    .ToList();
        }

        /// <summary>
        /// Builds the notify message.
        /// </summary>
        /// <param name="emailAddresses">The email addresses.</param>
        /// <param name="emailMessageType">Type of the email message.</param>
        /// <param name="personalisation">The personalisation.</param>
        /// <returns>The NotificationMessage.</returns>
        private NotificationMessage BuildNotifyMessage(IList<string> emailAddresses, string emailMessageType, IDictionary<string, object?> personalisation)
        {
            var notificationMessage = new NotificationMessage
            {
                EmailAddresses = emailAddresses,
                RequestingService = appSettings.RequestingService,
                EmailMessageType = emailMessageType,
                EmailPersonalisation = new GovUkNotifyPersonalisation
                {
                    Personalisation = personalisation,
                },
            };
            return notificationMessage;
        }
    }
}