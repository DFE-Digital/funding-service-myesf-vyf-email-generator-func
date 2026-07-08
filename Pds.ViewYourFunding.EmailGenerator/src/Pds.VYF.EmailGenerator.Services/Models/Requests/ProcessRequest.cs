// <copyright file="ProcessRequest.cs" company="Department for Education - Skill Funding Services">
// Copyright (c) Department for Education - Skill Funding Services. All rights reserved.
// </copyright>

using Pds.VYF.EmailGenerator.Services.Enumerations;

namespace Pds.VYF.EmailGenerator.Services.Models.Requests
{
    /// <summary>
    /// The class for ProcessRequest.
    /// </summary>
    public class ProcessRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessRequest"/> class.
        /// </summary>
        public ProcessRequest()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessRequest"/> class.
        /// </summary>
        /// <param name="emailTypes">The email types.</param>
        /// <param name="fundingStreamCode">The funding stream code.</param>
        /// <param name="fundingPeriodId">The funding period identifier.</param>
        public ProcessRequest(EmailTypesEnum emailTypes, string fundingStreamCode, string fundingPeriodId)
        {
            this.EmailTypes = emailTypes;
            this.FundingStreamCode = fundingStreamCode;
            this.FundingPeriodId = fundingPeriodId;
        }

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
        /// Gets or sets the funding stream name found.
        /// </summary>
        public string FundingStreamName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the funding period identifier.
        /// </summary>
        /// <value>
        /// The funding period identifier.
        /// </value>
        public string FundingPeriodId { get; set; } = default!;

        /// <summary>
        /// Gets or sets the digital statements go live date.
        /// </summary>
        /// <value>
        /// The digital statements go live date.
        /// </value>
        public string? DigitalStatementsGoLiveDate { get; set; }
    }
}