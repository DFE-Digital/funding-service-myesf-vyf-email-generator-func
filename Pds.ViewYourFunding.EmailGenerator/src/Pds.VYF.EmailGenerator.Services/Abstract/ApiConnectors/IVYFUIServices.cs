// <copyright file="IVYFUIServices.cs" company="Department for Education - Skill Funding Services">
// Copyright (c) Department for Education - Skill Funding Services. All rights reserved.
// </copyright>

using Pds.VYF.EmailGenerator.Services.Models.Requests;
using Pds.VYF.EmailGenerator.Services.Models.Responses;

namespace Pds.VYF.EmailGenerator.Services.Abstract.ApiConnectors
{
    /// <summary>
    /// Interface for IVYFUIServices.
    /// </summary>
    public interface IVYFUIServices
    {
        /// <summary>
        /// Gets the enabled funding stream and periods asynchronous.
        /// </summary>
        /// <returns>List of <see cref="EmailEnabledFundingStreamAndPeriodsResponse"></see>."/>.</returns>
        Task<List<EmailEnabledFundingStreamAndPeriodsResponse>?> GetEmailEnabledFundingStreamAndPeriodsAsync();

        /// <summary>
        /// Gets the latest funding stream published date.
        /// </summary>
        /// <param name="processRequest">The process request.</param>
        /// <returns>The latest publication date.</returns>
        Task<DateTime?> GetLatestFundingStreamPublishedDate(ProcessRequest processRequest);
    }
}
