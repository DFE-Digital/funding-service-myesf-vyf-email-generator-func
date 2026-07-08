// <copyright file="ProcessBatchRequest.cs" company="Department for Education - Skill Funding Services">
// Copyright (c) Department for Education - Skill Funding Services. All rights reserved.
// </copyright>

using Pds.VYF.EmailGenerator.Services.Enumerations;

namespace Pds.VYF.EmailGenerator.Services.Models.Requests
{
    /// <summary>
    /// The class for ProcessBatchRequest.
    /// </summary>
    public class ProcessBatchRequest
    {
        /// <summary>
        /// Gets or sets the email types.
        /// </summary>
        /// <value>
        /// The email types.
        /// </value>
        public EmailTypesEnum EmailTypes { get; set; }

        /// <summary>
        /// Gets or sets the funding stream code.
        /// </summary>
        /// <value>
        /// The funding stream code.
        /// </value>
        public string FundingStreamCode { get; set; } = default!;

        /// <summary>
        /// Gets or sets the funding period identifier.
        /// </summary>
        /// <value>
        /// The funding period identifier.
        /// </value>
        public string FundingPeriodId { get; set; } = default!;

        /// <summary>
        /// Gets or sets the status change date.
        /// </summary>
        /// <value>
        /// The status change date.
        /// </value>
        public string StatusChangeDate { get; set; } = default!;
    }
}