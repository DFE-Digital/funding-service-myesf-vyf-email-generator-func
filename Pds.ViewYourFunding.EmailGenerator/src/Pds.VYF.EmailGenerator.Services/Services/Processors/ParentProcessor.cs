// <copyright file="ParentProcessor.cs" company="Department for Education - Skill Funding Services">
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
using Pds.VYF.EmailGenerator.Services.Services.ApiConnectors;

namespace Pds.VYF.EmailGenerator.Services.Services.Processors
{
    /// <summary>
    /// The class for MultiAcademicTrustProcessor.
    /// </summary>
    /// <seealso cref="Pds.VYF.EmailGenerator.Services.Services.Processors.BaseProcessor&lt;Pds.VYF.EmailGenerator.Services.Models.CosmosModels.ParentCosmosModel, Pds.VYF.EmailGenerator.Services.Models.AzureTableModels.ParentAuditTableModel&gt;" />
    /// <seealso cref="Pds.VYF.EmailGenerator.Services.Abstract.Processors.IParentProcessor" />
    /// <remarks>
    /// Initializes a new instance of the <see cref="ParentProcessor" /> class.
    /// </remarks>
    /// <param name="logger">The logger.</param>
    /// <param name="cosmosContainerServices">The cosmos Container Services.</param>
    /// <param name="auditAndControlServices">The audit and control services.</param>
    /// <param name="azureTableSettingsOption">The azure table settings option.</param>
    /// <param name="appSettingsOptions">The application settings options.</param>
    /// <param name="cosmosQueryServices">The cosmos query services.</param>
    /// <param name="emailPublisher">The email publisher.</param>
    /// <param name="vYFUIServices">The VYF UI Services.</param>
    /// <exception cref="System.ArgumentNullException">All the params should not be null.</exception>
    public class ParentProcessor(
                    ILogger<ParentProcessor> logger,
                    ICosmosContainerServices cosmosContainerServices,
                    IAuditAndControlServices auditAndControlServices,
                    AzureTableSettings azureTableSettings,
                    AppSettings appSettings,
                    ICosmosQueryServices cosmosQueryServices,
                    IEmailPublisher emailPublisher,
                    IVYFUIServices vYFUIServices)
        : BaseProcessor<ParentCosmosModel, ParentAuditTableModel>(logger, auditAndControlServices, emailPublisher, vYFUIServices), IParentProcessor
    {
        /// <summary>
        /// Gets the name of the audit azure table.
        /// </summary>
        /// <value>
        /// The name of the audit azure table.
        /// </value>
        public override string AuditAzureTableName => azureTableSettings.ParentAuditTableName;

        /// <summary>
        /// Extracts the specified process request.
        /// </summary>
        /// <param name="processRequest">The process request.</param>
        /// <param name="statusChangeDate">The status change date.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A <see cref="Task" /> representing the asynchronous operation.
        /// </returns>
        public async override Task<List<ParentCosmosModel>> Extract(ProcessRequest processRequest, string statusChangeDate, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var process = "Extraction of Parent fundings from Cosmos is";

            logger.LogInformation(processRequest, "Extraction", $"{process} started!");

            var strSql = cosmosQueryServices.GetParentQuery(processRequest, statusChangeDate);
            var parentQueryResponse = await cosmosContainerServices.GetAsync<ParentCosmosModel>(CosmosContainerNameEnum.Funding, strSql, cancellationToken);

            logger.LogInformation(
                            processRequest,
                            "Extraction",
                            $"{process} completed successfully!",
                            parentQueryResponse.TimeTaken,
                            ("Extracted Items Count", parentQueryResponse.Count),
                            ("Total Request Charges", parentQueryResponse.RequestCharge));

            var parentsChunks = parentQueryResponse.Results.Chunk(appSettings.ParentSearchBatchSize);

            TimeSpan timeTaken = new TimeSpan(0, 0, 0);
            double requestCharges = 0;
            int childProvidersCount = 0;

            process = "Extraction of Child Providers under the Parent organization from Cosmos is";

            logger.LogInformation(processRequest, "Extraction", $"{process} started!");

            var sw = Stopwatch.StartNew();

            foreach (var parentsChunk in parentsChunks)
            {
                strSql = cosmosQueryServices.GetChildWithParentIdQuery(
                                                                    processRequest,
                                                                    statusChangeDate,
                                                                    parentsChunk.Select(a => a.Id));

                var childWithParentIdQueryResponse = await cosmosContainerServices
                                                                .GetAsync<ChildWithParentIdCosmosModel>(
                                                                            CosmosContainerNameEnum.ProviderFunding,
                                                                            strSql,
                                                                            cancellationToken);

                foreach (var parent in parentsChunk)
                {
                    var childrenWithNewFundings = childWithParentIdQueryResponse.Results
                                                                            .Where(child => child.ParentId == parent.Id
                                                                                                && (child.TypeOfFunding == TypeOfFundingEnum.New || child.TypeOfFunding == TypeOfFundingEnum.IndicativeNew))
                                                                            .Select(child => child.ProviderName)
                                                                            .OrderBy(childName => childName)
                                                                            .ToList();

                    var childrenWithUpdatedFundings = childWithParentIdQueryResponse.Results
                                                                            .Where(child => child.ParentId == parent.Id
                                                                                                && (child.TypeOfFunding == TypeOfFundingEnum.Updated || child.TypeOfFunding == TypeOfFundingEnum.IndicativeUpdated))
                                                                            .Select(child => child.ProviderName)
                                                                            .OrderBy(childName => childName)
                                                                            .ToList();

                    parent.NewProviderFundingCount = childrenWithNewFundings.Count;
                    parent.UpdatedProviderFundingCount = childrenWithUpdatedFundings.Count;

                    parent.ProvidersWithNewFunding = string.Join("\n", childrenWithNewFundings.Select((childName, index) => $"{index + 1}. {childName}"));
                    parent.ProvidersWithUpdatedFunding = string.Join("\n", childrenWithUpdatedFundings.Select((childName, index) => $"{index + 1}. {childName}"));
                }

                childProvidersCount += childWithParentIdQueryResponse.Count;
                requestCharges += childWithParentIdQueryResponse.RequestCharge;
                timeTaken += childWithParentIdQueryResponse.TimeTaken;
            }

            logger.LogInformation(
                            processRequest,
                            "Extraction",
                            $"{process} completed successfully!",
                            sw.Elapsed,
                            ("timeTakenSequentially", timeTaken),
                            ("Extracted Items Count", childProvidersCount),
                            ("Total Request Charges", requestCharges));

            return parentQueryResponse.Results;
        }

        /// <summary>
        /// Transforms the specified process request.
        /// </summary>
        /// <param name="processRequest">The process request.</param>
        /// <param name="cosmosModels">List of cosmos models.</param>
        /// <returns>
        /// A list of <see cref="!:TAuditTableModel" />.
        /// </returns>
        public override List<ParentAuditTableModel> Transform(ProcessRequest processRequest, List<ParentCosmosModel> cosmosModels)
        {
            var sw = Stopwatch.StartNew();
            var auditModels = cosmosModels.Select(cosmosModel => new ParentAuditTableModel(
                                                                        cosmosModel.FundingStreamCode,
                                                                        cosmosModel.FundingPeriodId,
                                                                        cosmosModel.UKPRN,
                                                                        cosmosModel.Id,
                                                                        cosmosModel.StatusChangedDate,
                                                                        cosmosModel.OrganizationName,
                                                                        cosmosModel.NewProviderFundingCount,
                                                                        cosmosModel.UpdatedProviderFundingCount,
                                                                        cosmosModel.ProvidersWithNewFunding ?? string.Empty,
                                                                        cosmosModel.ProvidersWithUpdatedFunding ?? string.Empty)).ToList();
            int emailToBeSentCount = 0, emailToBeSkippedCount = 0;

            foreach (var auditModel in auditModels)
            {
                if (auditModel.NewProviderFundingCount > 0 || auditModel.UpdatedProviderFundingCount > 0)
                {
                    auditModel.EmailPublishStatus = EmailPublishStatusEnum.InitialEntry;
                    emailToBeSentCount++;
                }
                else
                {
                    auditModel.EmailPublishStatus = EmailPublishStatusEnum.EmailSkipped;
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
        /// <exception cref="System.InvalidOperationException">No email required to send as there are no Provider data (this scenario will not occur as the Cosmos Query should handle this).</exception>
        public override string GetMessageType(ParentAuditTableModel auditEntity)
        {
            if (auditEntity.NewProviderFundingCount == 0 && auditEntity.UpdatedProviderFundingCount == 0)
            {
                throw new InvalidOperationException($"No email required to send as there are no Provider data (this scenario will not occur as the Cosmos Query should handle this).");
            }
            else if (auditEntity.NewProviderFundingCount > 0 && auditEntity.UpdatedProviderFundingCount > 0)
            {
                return EmailTemplatesModelExtension.GetEmailMessageType(a => a.ParentNewAndUpdatedFundings);
            }
            else if (auditEntity.NewProviderFundingCount > 0)
            {
                return EmailTemplatesModelExtension.GetEmailMessageType(a => a.ParentNewFundings);
            }
            else
            {
                return EmailTemplatesModelExtension.GetEmailMessageType(a => a.ParentUpdatedFundings);
            }
        }

        /// <summary>
        /// Gets the personalisation.
        /// </summary>
        /// <param name="auditEntity">The audit entity.</param>
        /// <returns>Personalization Message Dictionary.</returns>
        public override IDictionary<string, object?> GetPersonalisation(ParentAuditTableModel auditEntity)
        {
            var parentUILink = new Uri(appSettings.UIBaseUri).Append(appSettings.UIParentUrl).ToString();

            Dictionary<string, object?> personalisations = new()
            {
                { "fundingStream", auditEntity.FundingStreamCode.ToFundingStreamName() },
                { "ParentName", auditEntity.OrganizationName },
                { "totalNewandUpdatedStatementCount", auditEntity.NewProviderFundingCount + auditEntity.UpdatedProviderFundingCount },
                { "linktoallocationstatementspage", parentUILink },
            };

            if (auditEntity.NewProviderFundingCount > 0)
            {
                personalisations.TryAdd("newStatementCount", auditEntity.NewProviderFundingCount);
                personalisations.TryAdd("totalNewStatementCount", auditEntity.NewProviderFundingCount);
                personalisations.TryAdd("NStatements", auditEntity.NewProviderFundingCount > 1 ? "statements" : "statement");
                personalisations.TryAdd("NProviderNameList", auditEntity.ProvidersWithNewFunding);
            }

            if (auditEntity.UpdatedProviderFundingCount > 0)
            {
                personalisations.TryAdd("totalUpdatedStatementCount", auditEntity.UpdatedProviderFundingCount);
                personalisations.TryAdd("updatedStatementCount", auditEntity.UpdatedProviderFundingCount);
                personalisations.TryAdd("UStatements", auditEntity.UpdatedProviderFundingCount > 1 ? "statements" : "statement");
                personalisations.TryAdd("UProviderNameList", auditEntity.ProvidersWithUpdatedFunding);
            }

            return personalisations;
        }
    }
}
