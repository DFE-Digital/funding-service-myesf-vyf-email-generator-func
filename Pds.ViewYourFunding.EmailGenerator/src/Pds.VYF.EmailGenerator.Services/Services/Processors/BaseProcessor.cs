// <copyright file="BaseProcessor.cs" company="Department for Education - Skill Funding Services">
// Copyright (c) Department for Education - Skill Funding Services. All rights reserved.
// </copyright>

using Microsoft.Extensions.Logging;
using Pds.VYF.EmailGenerator.Services.Abstract.ApiConnectors;
using Pds.VYF.EmailGenerator.Services.Abstract.Controllers;
using Pds.VYF.EmailGenerator.Services.Abstract.Processors;
using Pds.VYF.EmailGenerator.Services.Enumerations;
using Pds.VYF.EmailGenerator.Services.Extensions;
using Pds.VYF.EmailGenerator.Services.Models.AzureTableModels;
using Pds.VYF.EmailGenerator.Services.Models.CosmosModels;
using Pds.VYF.EmailGenerator.Services.Models.Requests;
using System.Diagnostics;

namespace Pds.VYF.EmailGenerator.Services.Services.Processors
{
    /// <summary>
    /// Base Class for Email Core Processor.
    /// </summary>
    /// <typeparam name="TCosmosModel">Any class which inherits from <see cref="BaseCosmosModel"/>.</typeparam>
    /// <typeparam name="TAuditTableModel">Any class which inherits from <see cref="BaseAuditTableModel"/>.</typeparam>
    /// <seealso cref="Pds.VYF.EmailGenerator.Services.Abstract.Processors.IBaseProcessor" />
    /// <remarks>
    /// Initializes a new instance of the <see cref="BaseProcessor{TCosmosModel, TAuditTableModel}"/> class.
    /// </remarks>
    /// <param name="logger">The logger.</param>
    /// <param name="auditAndControlServices">The audit and control services.</param>
    /// <param name="emailPublisher">The email publisher.</param>
    /// <param name="vYFUIServices">The VYF UI Services.</param>
    /// <exception cref="System.ArgumentNullException">Params should not be null.</exception>
    public abstract class BaseProcessor<TCosmosModel, TAuditTableModel>(
                                                ILogger<BaseProcessor<TCosmosModel, TAuditTableModel>> logger,
                                                IAuditAndControlServices auditAndControlServices,
                                                IEmailPublisher emailPublisher,
                                                IVYFUIServices vYFUIServices)
        : IBaseProcessor
        where TCosmosModel : BaseCosmosModel
        where TAuditTableModel : BaseAuditTableModel
    {
        /// <summary>
        /// Gets the name of the audit azure table.
        /// </summary>
        /// <value>
        /// The name of the audit azure table.
        /// </value>
        public abstract string AuditAzureTableName { get; }

        /// <summary>
        /// Processes the asynchronous.
        /// </summary>
        /// <param name="processRequest">The process request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task ProcessAsync(ProcessRequest processRequest, CancellationToken cancellationToken)
        {
            await this.ProcessUnsendEmailsInternalAsync(processRequest, cancellationToken);

            var newSourceEntities = await this.ExtractNewFundingsInternalAsync(processRequest, cancellationToken);

            List<TAuditTableModel> auditEntries = this.TransformInternal(processRequest, newSourceEntities, cancellationToken);

            await auditAndControlServices.InitiateAuditAsync(processRequest, auditEntries, this.AuditAzureTableName, cancellationToken);

            await this.ProcessEmailInternalAsync(processRequest, auditEntries.Where(a => a.EmailPublishStatus == EmailPublishStatusEnum.InitialEntry), cancellationToken);
        }

        /// <summary>
        /// Extracts the specified process request.
        /// </summary>
        /// <param name="processRequest">The process request.</param>
        /// <param name="statusChangeDate">The status change date.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A <see cref="Task" /> representing the asynchronous operation.
        /// </returns>
        public abstract Task<List<TCosmosModel>> Extract(ProcessRequest processRequest, string statusChangeDate, CancellationToken cancellationToken);

        /// <summary>
        /// Transforms the specified process request.
        /// </summary>
        /// <param name="processRequest">The process request.</param>
        /// <param name="cosmosModels">List of cosmos models.</param>
        /// <returns>A list of <see cref="TAuditTableModel"/>.</returns>
        public abstract List<TAuditTableModel> Transform(ProcessRequest processRequest, List<TCosmosModel> cosmosModels);

        /// <summary>
        /// Gets the personalisation.
        /// </summary>
        /// <param name="auditEntity">The audit entity.</param>
        /// <returns>A dictionary of Personalisation.</returns>
        public abstract IDictionary<string, object?> GetPersonalisation(TAuditTableModel auditEntity);

        /// <summary>
        /// Gets the type of the message.
        /// </summary>
        /// <param name="auditEntity">The audit entity.</param>
        /// <returns>Message Type.</returns>
        public abstract string GetMessageType(TAuditTableModel auditEntity);

        // Private Methods

        /// <summary>
        /// Processes the email internal asynchronous.
        /// </summary>
        /// <param name="processRequest">The process request.</param>
        /// <param name="auditEntries">The audit entries.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        private async Task<int> ProcessEmailInternalAsync(ProcessRequest processRequest, IEnumerable<TAuditTableModel> auditEntries, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await emailPublisher.PublishAsync(processRequest, auditEntries, this.GetMessageType, this.GetPersonalisation, this.AuditAzureTableName, cancellationToken);
        }

        /// <summary>
        /// Process all Unsend Emails (which might be failed in last run or manually requested for rerun).
        /// </summary>
        /// <param name="processRequest">The process request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        private async Task ProcessUnsendEmailsInternalAsync(ProcessRequest processRequest, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Process Unsend Emails (for Resend or Unprocessed from last run)
            var sw = Stopwatch.StartNew();

            var unsendAuditEntriesForResendOrUnprocessed = await auditAndControlServices.GetUnsendAuditEntriesAsync<TAuditTableModel>(
                                                                                                processRequest,
                                                                                                this.AuditAzureTableName,
                                                                                                EmailPublishStatusEnum.InitialEntry,
                                                                                                cancellationToken);

            int emailsPublishedCountForResendOrUnprocessed = await this.ProcessEmailInternalAsync(processRequest, unsendAuditEntriesForResendOrUnprocessed, cancellationToken);

            logger.LogInformation(
                            processRequest,
                            "Unsend Email Processor",
                            $"Processing new emails (Requested manually to resent) completed Successfully!",
                            sw.Elapsed,
                            (nameof(emailsPublishedCountForResendOrUnprocessed), emailsPublishedCountForResendOrUnprocessed),
                            ("Total Expected Count", unsendAuditEntriesForResendOrUnprocessed.Count));

            // Process Unsend Emails (for previously failed)
            sw.Restart();

            var unsendAuditEntriesForFailed = await auditAndControlServices.GetUnsendAuditEntriesAsync<TAuditTableModel>(
                                                                                                processRequest,
                                                                                                this.AuditAzureTableName,
                                                                                                EmailPublishStatusEnum.EmailFailedToPublish,
                                                                                                cancellationToken);

            int emailsPublishedCountOfPreviouslyFailed = await this.ProcessEmailInternalAsync(processRequest, unsendAuditEntriesForFailed, cancellationToken);

            logger.LogInformation(
                            processRequest,
                            "Unsend Email Processor",
                            $"Processing previously failed emails completed Successfully!",
                            sw.Elapsed,
                            (nameof(emailsPublishedCountOfPreviouslyFailed), emailsPublishedCountOfPreviouslyFailed),
                            ("Total Expected Count", unsendAuditEntriesForFailed.Count));
        }

        /// <summary>
        /// Extracts the new fundings internal asynchronous.
        /// </summary>
        /// <param name="processRequest">The process request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        private async Task<List<TCosmosModel>> ExtractNewFundingsInternalAsync(ProcessRequest processRequest, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var sw = Stopwatch.StartNew();

            var process = "Extraction steps are";

            logger.LogInformation(processRequest, "Extraction", $"{process} started!");

            var lastSuccessfullyProcessedStatusChangeDate = await auditAndControlServices.GetLastStatusChangeDate(processRequest, cancellationToken);

            var newFundings = await this.Extract(processRequest, lastSuccessfullyProcessedStatusChangeDate, cancellationToken);

            logger.LogInformation(
                        processRequest,
                        "Extraction",
                        $"{process} completed Successfully!",
                        sw.Elapsed,
                        ("Total Extracted Items Count", newFundings.Count));
            return newFundings;
        }

        /// <summary>
        /// Extracts the internal.
        /// </summary>
        /// <param name="processRequest">The process request.</param>
        /// <param name="newSourceEntities">The new source entities.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>List of <see cref="TAuditTableModel"/>.</returns>
        private List<TAuditTableModel> TransformInternal(ProcessRequest processRequest, List<TCosmosModel> newSourceEntities, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return this.Transform(processRequest, newSourceEntities);
        }
    }
}
