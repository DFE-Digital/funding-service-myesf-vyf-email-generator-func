// <copyright file="AuditAndControlServices.cs" company="Department for Education - Skill Funding Services">
// Copyright (c) Department for Education - Skill Funding Services. All rights reserved.
// </copyright>

using System.Diagnostics;
using Azure;
using Microsoft.Extensions.Logging;
using Pds.VYF.EmailGenerator.Services.Abstract.Controllers;
using Pds.VYF.EmailGenerator.Services.Abstract.InfraServices;
using Pds.VYF.EmailGenerator.Services.Enumerations;
using Pds.VYF.EmailGenerator.Services.Extensions;
using Pds.VYF.EmailGenerator.Services.Models.AzureTableModels;
using Pds.VYF.EmailGenerator.Services.Models.ConfigurationSettings;
using Pds.VYF.EmailGenerator.Services.Models.Requests;

namespace Pds.VYF.EmailGenerator.Services.Services.Controllers
{
    /// <summary>
    /// The class for AuditAndControlServices.
    /// </summary>
    /// <seealso cref="Pds.VYF.EmailGenerator.Services.Abstract.Controllers.IAuditAndControlServices" />
    /// <remarks>
    /// Initializes a new instance of the <see cref="AuditAndControlServices" /> class.
    /// </remarks>
    /// <param name="logger">The logger.</param>
    /// <param name="azureTableSettings">The azure table settings.</param>
    /// <param name="azureTableServices">The azure table services.</param>
    /// <exception cref="System.ArgumentNullException">Should have valid Params.</exception>
    public class AuditAndControlServices(
                            ILogger<AuditAndControlServices> logger,
                            AzureTableSettings azureTableSettings,
                            IAzureTableServices azureTableServices) : IAuditAndControlServices
    {
        private readonly string pkLastSuccessfullyProcessedStatusChangeDate = "LastSuccessfullyProcessedStatusChangeDate";

        /// <summary>
        /// Gets the last status change date.
        /// </summary>
        /// <param name="processRequest">The process request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<string> GetLastStatusChangeDate(ProcessRequest processRequest, CancellationToken cancellationToken)
        {
            var rowKey = GetVYFEmailControlRowId(processRequest);
            var result = await azureTableServices.Query<ControlTableModel>(azureTableSettings.ControlTableName, a => a.PartitionKey == this.pkLastSuccessfullyProcessedStatusChangeDate && a.RowKey == rowKey, cancellationToken: cancellationToken);

            var lastStatusChangesDate = result.FirstOrDefault()?.StatusChangedDate ?? "1900-01-01 00:00:00";

            logger.LogInformation(processRequest, "Extraction", $"The last extracted status changes date is {lastStatusChangesDate}.");

            return lastStatusChangesDate;
        }

        /// <summary>
        /// Upserts the control entry asynchronous.
        /// </summary>
        /// <param name="processRequest">The process request.</param>
        /// <param name="statusChangeDate">The status change date.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task UpsertControlEntryAsync(ProcessRequest processRequest, string statusChangeDate, CancellationToken cancellationToken)
        {
            var lastProcessedEntity = new ControlTableModel(
                                                    this.pkLastSuccessfullyProcessedStatusChangeDate,
                                                    GetVYFEmailControlRowId(processRequest))
            {
                StatusChangedDate = statusChangeDate,
            };
            await azureTableServices.UpsertEntityAsync(azureTableSettings.ControlTableName, lastProcessedEntity, cancellationToken);
        }

        /// <summary>
        /// Upserts the audit entry asynchronous.
        /// </summary>
        /// <param name="processRequest">The process request.</param>
        /// <param name="auditTableName">Name of the audit table.</param>
        /// <param name="auditEntity">The audit entity.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<int> UpsertAuditEntryAsync(ProcessRequest processRequest, string auditTableName, BaseAuditTableModel auditEntity, CancellationToken cancellationToken)
        {
            try
            {
                var count = await azureTableServices.UpsertEntityAsync(auditTableName, auditEntity, cancellationToken);
                return count;
            }
            catch (RequestFailedException ex)
            {
                logger.LogError(processRequest, "Email Publisher", ex, $"Failed to update the audit data for the audit table: {auditTableName} with error code: {ex.Status}, error message: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Gets the unsend audit entries asynchronous.
        /// </summary>
        /// <typeparam name="TAuditTableModel">Any class which inherit from <see cref="BaseAuditTableModel">BaseAuditTableModel</see>.</typeparam>
        /// <param name="processRequest">The process request.</param>
        /// <param name="auditTableName">Name of the audit table.</param>
        /// <param name="emailPublishStatusToBeQueried">The email publish status to be queried.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<List<TAuditTableModel>> GetUnsendAuditEntriesAsync<TAuditTableModel>(ProcessRequest processRequest, string auditTableName, EmailPublishStatusEnum emailPublishStatusToBeQueried, CancellationToken cancellationToken)
            where TAuditTableModel : BaseAuditTableModel
        {
            var sw = Stopwatch.StartNew();
            var result = await azureTableServices.Query<TAuditTableModel>(
                auditTableName,
                a => a.FundingStreamCode == processRequest.FundingStreamCode
                            && a.FundingPeriodId == processRequest.FundingPeriodId
                            && a.EmailPublishStatus.Equals(emailPublishStatusToBeQueried.ToString()),
                null,
                cancellationToken);

            logger.LogInformation(processRequest, "Unsend Email Processor", $"Finding unsend email details identified from the table [{auditTableName}] successfully!", sw.Elapsed, ("Total Count", result.Count()));
            sw.Stop();

            return result;
        }

        /// <summary>
        /// Initiates the audit asynchronous.
        /// </summary>
        /// <typeparam name="TAuditTableModel">Any class which inherit from <see cref="BaseAuditTableModel">BaseAuditTableModel</see>.</typeparam>
        /// <param name="processRequest">The process request.</param>
        /// <param name="auditEntities">The audit entities.</param>
        /// <param name="auditTableName">Name of the audit table.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task InitiateAuditAsync<TAuditTableModel>(ProcessRequest processRequest, List<TAuditTableModel> auditEntities, string auditTableName, CancellationToken cancellationToken)
            where TAuditTableModel : BaseAuditTableModel
        {
            cancellationToken.ThrowIfCancellationRequested();

            var sw = Stopwatch.StartNew();

            var groupedAuditEntities = auditEntities
                        .GroupBy(item => item.StatusChangedDate ?? string.Empty)
                        .OrderBy(a => a.Key)
                        .ToDictionary(a => a.Key, a => a.ToList());

            int batchNumber = 0, totalAuditEntries = 0, succeedAuditEntries = 0, failedAuditEntries = 0;

            bool isLastStatusChangeDateUploadedSuccessfully = true;

            foreach (var auditEntitiesPerStatusChangeDate in groupedAuditEntities)
            {
                int insertedRows = 0, failedRows = auditEntitiesPerStatusChangeDate.Value.Count, skippedCount = 0;
                batchNumber++;
                cancellationToken.ThrowIfCancellationRequested();

                var existingAuditEntries = await azureTableServices.Query<TAuditTableModel>(
                    auditTableName,
                    a => a.FundingStreamCode == processRequest.FundingStreamCode
                                && a.FundingPeriodId == processRequest.FundingPeriodId
                                && a.StatusChangedDate == auditEntitiesPerStatusChangeDate.Key,
                    null,
                    cancellationToken);

                skippedCount = existingAuditEntries.Count();
                failedRows -= skippedCount;

                var auditEntriesToBeInserted = auditEntitiesPerStatusChangeDate.Value.Where(a => !existingAuditEntries.Any(b => b.RowKey == a.RowKey)).ToList();

                try
                {
                    insertedRows = await azureTableServices.UpsertEntitiesAsync(auditTableName, auditEntriesToBeInserted, cancellationToken);
                    failedRows -= insertedRows;
                }
                catch (Exception)
                {
                    // ToDo: Check if Exception needs to be handled?
                }

                logger.LogInformation(
                    processRequest,
                    "Audit initiator",
                    $"Audit Entries Inserted/Updated into the table {auditTableName} Successfully!",
                    sw.Elapsed,
                    (nameof(batchNumber), batchNumber),
                    ("StatusChangeDate", auditEntitiesPerStatusChangeDate.Key),
                    (nameof(insertedRows), insertedRows),
                    (nameof(failedRows), failedRows));

                succeedAuditEntries += insertedRows;
                failedAuditEntries += failedRows;
                totalAuditEntries += auditEntitiesPerStatusChangeDate.Value.Count;

                if (failedAuditEntries == 0 && isLastStatusChangeDateUploadedSuccessfully)
                {
                    try
                    {
                        await this.UpsertControlEntryAsync(processRequest, auditEntitiesPerStatusChangeDate.Key, cancellationToken);
                    }
                    catch (Exception)
                    {
                        isLastStatusChangeDateUploadedSuccessfully = false;
                    }
                }
            }

            var statusMessage = totalAuditEntries == succeedAuditEntries ? "fully completed!"
                    : totalAuditEntries == failedAuditEntries ? "fully failed!"
                    : "partially completed";
            string message = $"The Initiate Audit Entries Operation is {statusMessage}";
            logger.LogInformation(processRequest, "Audit initiator", message, sw.Elapsed, (nameof(succeedAuditEntries), succeedAuditEntries), (nameof(failedAuditEntries), failedAuditEntries));
        }

        /// <summary>
        /// Gets the vyf email control row identifier.
        /// </summary>
        /// <param name="processRequest">The process request.</param>
        /// <returns>The Row Id of VYF Email Control Audit Table.</returns>
        private static string GetVYFEmailControlRowId(ProcessRequest processRequest)
        {
            return $"{processRequest.EmailTypes}-{processRequest.FundingStreamCode}-{processRequest.FundingPeriodId}";
        }
    }
}