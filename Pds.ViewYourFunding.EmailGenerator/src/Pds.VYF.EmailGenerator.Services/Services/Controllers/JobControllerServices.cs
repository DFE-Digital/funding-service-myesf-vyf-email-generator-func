// <copyright file="JobControllerServices.cs" company="Department for Education - Skill Funding Services">
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
using Pds.VYF.EmailGenerator.Services.Models.Requests;

namespace Pds.VYF.EmailGenerator.Services.Services.Controllers
{
    /// <summary>
    /// The class for JobControllerServices.
    /// </summary>
    /// <seealso cref="Pds.VYF.EmailGenerator.Services.Abstract.Controllers.IJobControllerServices" />
    /// <remarks>
    /// Initializes a new instance of the <see cref="JobControllerServices" /> class.
    /// </remarks>
    /// <param name="logger">The logger.</param>
    /// <param name="vYFUIServices">The VYF UI services.</param>
    /// <param name="parentProcessor">The parent processor.</param>
    /// <param name="childProcessor">The child processor.</param>
    /// <param name="ICosmosContainerServices">The Cosmos Container Services.</param>
    /// <param name="ICosmosQueryServices">The Cosmos Query Services.</param>
    public class JobControllerServices(
                            ILogger<JobControllerServices> logger,
                            IVYFUIServices vYFUIServices,
                            IParentProcessor parentProcessor,
                            IChildProcessor childProcessor,
                            ICosmosContainerServices cosmosContainerServices,
                            ICosmosQueryServices cosmosQueryServices) : IJobControllerServices
    {
        /// <summary>
        /// Runs the asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task RunAsync(CancellationToken cancellationToken)
        {
            if (await this.IsFeedReaderRunning(cancellationToken))
            {
                logger.LogInformation("A Feed Reader Job is already in progress. The current Email Job will be stopped without taking any further actions.");
                return;
            }

            var enabledFundingStreamAndPeriodsList = await vYFUIServices.GetEmailEnabledFundingStreamAndPeriodsAsync();

            if (enabledFundingStreamAndPeriodsList == null
                    || enabledFundingStreamAndPeriodsList.Count == 0
                    || enabledFundingStreamAndPeriodsList.SelectMany(a => a.FundingPeriods).All(b => string.IsNullOrWhiteSpace(b)))
            {
                logger.LogInformation("There are no Funding stream/period enabled.");
                return;
            }

            foreach (var enabledFundingStreamAndPeriods in enabledFundingStreamAndPeriodsList)
            {
                if (enabledFundingStreamAndPeriods.DigitalStatementsGoLiveDate == null || enabledFundingStreamAndPeriods.DigitalStatementsGoLiveDate.Value < new DateTime(2024, 05, 01))
                {
                    logger.LogError($"Invalid Digital Go-Live date was provided for the funding stream: {enabledFundingStreamAndPeriods.FundingStreamName}. The Go-Live date should be after 1st May 2024.");
                    continue;
                }

                if (enabledFundingStreamAndPeriods.HasChildViewEnabled)
                {
                    foreach (var fundingPeriod in enabledFundingStreamAndPeriods.FundingPeriods.Where(b => !string.IsNullOrWhiteSpace(b)))
                    {
                        await this.RunForAFundingStreamAndPeriod(
                                                        EmailTypesEnum.ForChildren,
                                                        enabledFundingStreamAndPeriods.FundingStreamCode,
                                                        enabledFundingStreamAndPeriods.FundingStreamName,
                                                        fundingPeriod,
                                                        enabledFundingStreamAndPeriods.DigitalStatementsGoLiveDate,
                                                        cancellationToken);
                    }
                }

                if (enabledFundingStreamAndPeriods.HasParentViewEnabled)
                {
                    foreach (var fundingPeriod in enabledFundingStreamAndPeriods.FundingPeriods.Where(b => !string.IsNullOrWhiteSpace(b)))
                    {
                        await this.RunForAFundingStreamAndPeriod(
                                                        EmailTypesEnum.ForParents,
                                                        enabledFundingStreamAndPeriods.FundingStreamCode,
                                                        enabledFundingStreamAndPeriods.FundingStreamName,
                                                        fundingPeriod,
                                                        enabledFundingStreamAndPeriods.DigitalStatementsGoLiveDate,
                                                        cancellationToken);
                    }
                }
            }
        }

        /// <summary>
        /// Runs for a funding stream and period.
        /// </summary>
        /// <param name="emailTypesEnum">The email types enum.</param>
        /// <param name="fundingStreamCode">The funding stream code.</param>
        /// <param name="fundingStreamName">Name of the funding stream.</param>
        /// <param name="fundingPeriodId">The funding period identifier.</param>
        /// <param name="digitalStatementsGoLiveDate">The digital statements go live date.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A <see cref="Task" /> representing the asynchronous operation.
        /// </returns>
        public async Task RunForAFundingStreamAndPeriod(
                                                        EmailTypesEnum emailTypesEnum,
                                                        string fundingStreamCode,
                                                        string fundingStreamName,
                                                        string fundingPeriodId,
                                                        DateTime? digitalStatementsGoLiveDate,
                                                        CancellationToken cancellationToken)
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                IBaseProcessor processor = emailTypesEnum == EmailTypesEnum.ForChildren ? childProcessor : parentProcessor;
                var processRequest = new ProcessRequest(
                                                emailTypesEnum,
                                                fundingStreamCode,
                                                fundingPeriodId)
                {
                    FundingStreamName = fundingStreamName,
                    DigitalStatementsGoLiveDate = digitalStatementsGoLiveDate?.ToString("yyyy-MM-ddTHH:mm:ss+00:00"),
                };

                var sw = Stopwatch.StartNew();
                logger.LogInformation(processRequest, "Job Controller", $"Process started for {processRequest.FundingStreamCode}-{processRequest.FundingPeriodId} successfully.");

                try
                {
                    await processor.ProcessAsync(processRequest, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    logger.LogInformation("Cancellation requested by function App Host and application gracefully exited!");
                    return;
                }
                catch (Exception ex)
                {
                    logger.LogError(processRequest, "Job Controller", ex);
                    return;
                }

                logger.LogInformation(processRequest, "Job Controller", $"Process completed for {processRequest.FundingStreamCode}-{processRequest.FundingPeriodId}-{processRequest.EmailTypes} successfully.", sw.Elapsed);
            }
        }

        /// <summary>
        /// Determines whether [is feed reader running] [the specified cancellation token].
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        ///   <c>true</c> if [is feed reader running] [the specified cancellation token]; otherwise, <c>false</c>.
        /// </returns>
        private async Task<bool> IsFeedReaderRunning(CancellationToken cancellationToken)
        {
            var strSql = cosmosQueryServices.GetLastFeedReaderAuditQuery();

            var lastRunStatus = await cosmosContainerServices.GetAsync<string>(CosmosContainerNameEnum.Audit, strSql, cancellationToken);

            return lastRunStatus.Results.Any(a => string.Equals(a, "started", StringComparison.OrdinalIgnoreCase));
        }
    }
}