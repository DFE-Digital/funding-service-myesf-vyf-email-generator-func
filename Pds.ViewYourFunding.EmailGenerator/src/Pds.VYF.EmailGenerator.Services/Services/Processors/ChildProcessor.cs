// <copyright file="ChildProcessor.cs" company="Department for Education - Skill Funding Services">
// Copyright (c) Department for Education - Skill Funding Services. All rights reserved.
// </copyright>

using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Pds.VYF.EmailGenerator.Services.Abstract.ApiConnectors;
using Pds.VYF.EmailGenerator.Services.Abstract.Controllers;
using Pds.VYF.EmailGenerator.Services.Abstract.InfraServices;
using Pds.VYF.EmailGenerator.Services.Abstract.Processors;
using Pds.VYF.EmailGenerator.Services.Enumerations;
using Pds.VYF.EmailGenerator.Services.Extensions;
using Pds.VYF.EmailGenerator.Services.Helpers;
using Pds.VYF.EmailGenerator.Services.Models.AzureTableModels;
using Pds.VYF.EmailGenerator.Services.Models.ConfigurationSettings;
using Pds.VYF.EmailGenerator.Services.Models.CosmosModels;
using Pds.VYF.EmailGenerator.Services.Models.Requests;

namespace Pds.VYF.EmailGenerator.Services.Services.Processors
{
    /// <summary>
    /// The class for ChildProcessor.
    /// </summary>
    /// <seealso cref="Pds.VYF.EmailGenerator.Services.Services.Processors.BaseProcessor&lt;Pds.VYF.EmailGenerator.Services.Models.CosmosModels.ChildCosmosModel, Pds.VYF.EmailGenerator.Services.Models.AzureTableModels.ChildAuditTableModel&gt;" />
    /// <seealso cref="Pds.VYF.EmailGenerator.Services.Abstract.Processors.IChildProcessor" />
    /// <remarks>
    /// Initializes a new instance of the <see cref="ChildProcessor" /> class.
    /// </remarks>
    /// <param name="logger">The logger.</param>
    /// <param name="cosmosClientResolver">The cosmos client resolver.</param>
    /// <param name="auditAndControlServices">The audit and control services.</param>
    /// <param name="azureTableSettings">The azure table settings.</param>
    /// <param name="appSettings">The application settings.</param>
    /// <param name="cosmosQueryServices">The cosmos query services.</param>
    /// <param name="emailPublisher">The email publisher.</param>
    /// <param name="vYFUIServices">The VYF UI Services.</param>
    public class ChildProcessor(
                    ILogger<ChildProcessor> logger,
                    ICosmosContainerServices cosmosContainerServices,
                    IAuditAndControlServices auditAndControlServices,
                    AzureTableSettings azureTableSettings,
                    AppSettings appSettings,
                    ICosmosQueryServices cosmosQueryServices,
                    IEmailPublisher emailPublisher,
                    IVYFUIServices vYFUIServices)
        : BaseProcessor<ChildCosmosModel, ChildAuditTableModel>(logger, auditAndControlServices, emailPublisher, vYFUIServices), IChildProcessor
    {
        /// <summary>
        /// Gets the name of the audit azure table.
        /// </summary>
        /// <value>
        /// The name of the audit azure table.
        /// </value>
        public override string AuditAzureTableName => azureTableSettings.ChildAuditTableName;

        /// <summary>
        /// Extracts the specified process request.
        /// </summary>
        /// <param name="processRequest">The process request.</param>
        /// <param name="statusChangeDate">The status change date.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A <see cref="T:System.Threading.Tasks.Task" /> representing the asynchronous operation.
        /// </returns>
        public async override Task<List<ChildCosmosModel>> Extract(ProcessRequest processRequest, string statusChangeDate, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var strSql = cosmosQueryServices.GetChildQuery(processRequest, statusChangeDate);
            var response = await cosmosContainerServices.GetAsync<ChildCosmosModel>(CosmosContainerNameEnum.ProviderFunding, strSql, cancellationToken);

            logger.LogInformation(
                            processRequest,
                            "Extraction",
                            "New Data Extracted from Cosmos completed successfully!",
                            response.TimeTaken,
                            ("Extracted Items Count", response.Count),
                            ("Total Request Charges", response.RequestCharge));

            return response.Results;
        }

        /// <summary>
        /// Transforms the specified process request.
        /// </summary>
        /// <param name="processRequest">The process request.</param>
        /// <param name="cosmosModels">The cosmos Models.</param>
        /// <returns>A list of <see cref="ChildAuditTableModel"/>.</returns>
        public override List<ChildAuditTableModel> Transform(ProcessRequest processRequest, List<ChildCosmosModel> cosmosModels)
        {
            var sw = Stopwatch.StartNew();
            var auditModels = cosmosModels.Select(cosmosModel => new ChildAuditTableModel(
                                            cosmosModel.FundingStreamCode,
                                            cosmosModel.FundingPeriodId,
                                            cosmosModel.UKPRN,
                                            cosmosModel.Id,
                                            cosmosModel.StatusChangedDate,
                                            cosmosModel.ProviderName,
                                            cosmosModel.TypeOfFunding)).ToList();
            int emailToBeSentCount = 0, emailToBeSkippedCount = 0;

            foreach (var groupedItem in auditModels.GroupBy(item => item.UKPRN))
            {
                var sortedItems = groupedItem.OrderByDescending(item => item.StatusChangedDate);
                sortedItems.First().EmailPublishStatus = EmailPublishStatusEnum.InitialEntry;
                emailToBeSentCount++;

                foreach (var item in sortedItems.Skip(1))
                {
                    item.EmailPublishStatus = EmailPublishStatusEnum.EmailSkipped;
                    emailToBeSkippedCount++;
                }
            }

            logger.LogInformation(
                processRequest,
                "Transformer",
                $"Transforming Cosmos to Audit data is Successfully completed!",
                sw.Elapsed,
                (nameof(emailToBeSentCount), emailToBeSentCount),
                (nameof(emailToBeSkippedCount), emailToBeSkippedCount));

            return auditModels;
        }

        /// <summary>
        /// Gets the type of the message.
        /// </summary>
        /// <param name="auditEntity">The audit entity.</param>
        /// <returns>Message Type.</returns>
        public override string GetMessageType(ChildAuditTableModel auditEntity)
        {
            if (auditEntity.TypeOfFunding == TypeOfFundingEnum.New || auditEntity.TypeOfFunding == TypeOfFundingEnum.IndicativeNew)
            {
                return EmailTemplatesModelExtension.GetEmailMessageType(a => a.ChildNewFunding);
            }
            else
            {
                return EmailTemplatesModelExtension.GetEmailMessageType(a => a.ChildUpdatedFunding);
            }
        }

        /// <summary>
        /// Gets the personalisation.
        /// </summary>
        /// <param name="auditEntity">The audit entity.</param>
        /// <returns>Personalisations.</returns>
        public override IDictionary<string, object?> GetPersonalisation(ChildAuditTableModel auditEntity)
        {
            var childUILink = new Uri(appSettings.UIBaseUri).Append(appSettings.UIChildUrl).ToString();

            childUILink = string.Format(
                                    childUILink,
                                    auditEntity.UKPRN,
                                    auditEntity.FundingStreamCode.ToFundingStreamNameForChildUrl(),
                                    auditEntity.StatusChangedDate?.FormatStatusChangeDateForChildUrl() ?? "1-1-1900",
                                    auditEntity.FundingPeriodId.GetStartYear(),
                                    auditEntity.FundingPeriodId.GetEndYear());

            return new Dictionary<string, object?>()
            {
                { "ProviderName", auditEntity.ProviderName },
                { "fundingStream", auditEntity.FundingStreamCode.ToFundingStreamName() },
                { "linktospecificallocationstatement", childUILink },
            };
        }
    }
}
