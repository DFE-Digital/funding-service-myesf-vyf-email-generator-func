// <copyright file="IJobControllerServices.cs" company="Department for Education - Skill Funding Services">
// Copyright (c) Department for Education - Skill Funding Services. All rights reserved.
// </copyright>

using Pds.VYF.EmailGenerator.Services.Enumerations;

namespace Pds.VYF.EmailGenerator.Services.Abstract.Controllers
{
    /// <summary>
    /// The interface for IJobControllerServices.
    /// </summary>
    public interface IJobControllerServices
    {
        /// <summary>
        /// Runs the asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task RunAsync(CancellationToken cancellationToken);

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
        Task RunForAFundingStreamAndPeriod(
            EmailTypesEnum emailTypesEnum,
            string fundingStreamCode,
            string fundingStreamName,
            string fundingPeriodId,
            DateTime? digitalStatementsGoLiveDate,
            CancellationToken cancellationToken);
    }
}
