// <copyright file="BaseAuditTableModel.cs" company="Department for Education - Skill Funding Services">
// Copyright (c) Department for Education - Skill Funding Services. All rights reserved.
// </copyright>

using Pds.VYF.EmailGenerator.Services.Enumerations;

namespace Pds.VYF.EmailGenerator.Services.Models.AzureTableModels
{
    /// <summary>
    /// A class for BaseAuditTableModel.
    /// </summary>
    /// <seealso cref="Pds.VYF.EmailGenerator.Services.Models.AzureTableModels.BaseTableEntity" />
    public abstract class BaseAuditTableModel : BaseTableEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseAuditTableModel"/> class.
        /// </summary>
        /// <param name="fundingStreamCode">The funding stream code.</param>
        /// <param name="fundingPeriodId">The funding period identifier.</param>
        /// <param name="ukprn">The ukprn.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="statusChangedDate">The status changed date.</param>
        /// <param name="typeOfFunding">The type of funding.</param>
        public BaseAuditTableModel(
                                string fundingStreamCode,
                                string fundingPeriodId,
                                string ukprn,
                                string id,
                                string statusChangedDate)
            : base($"{fundingStreamCode}-{fundingPeriodId}", id)
        {
            this.FundingStreamCode = fundingStreamCode;
            this.FundingPeriodId = fundingPeriodId;
            this.UKPRN = ukprn;
            this.StatusChangedDate = statusChangedDate;
            this.EmailPublishStatus = EmailPublishStatusEnum.InitialEntry;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseAuditTableModel"/> class.
        /// </summary>
        protected BaseAuditTableModel()
        {
        }

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
        /// Gets or sets the ukprn.
        /// </summary>
        /// <value>
        /// The ukprn.
        /// </value>
        public string UKPRN { get; set; } = default!;

        /// <summary>
        /// Gets or sets the status changed date.
        /// </summary>
        /// <value>
        /// The status changed date.
        /// </value>
        public string? StatusChangedDate { get; set; }

        /// <summary>
        /// Gets the type of the email.
        /// </summary>
        /// <value>
        /// The type of the email.
        /// </value>
        public abstract EmailTypesEnum EmailType { get; }

        /// <summary>
        /// Gets or sets the email publish status.
        /// </summary>
        /// <value>
        /// The email publish status.
        /// </value>
        public EmailPublishStatusEnum EmailPublishStatus { get; set; } = default!;

        /// <summary>
        /// Gets or sets the email addresses.
        /// </summary>
        /// <value>
        /// The email addresses.
        /// </value>
        public string? EmailAddresses { get; set; }

        /// <summary>
        /// Gets or sets the email published at.
        /// </summary>
        /// <value>
        /// The email published at.
        /// </value>
        public string? EmailPublishedAt { get; set; }

        /// <summary>
        /// Gets or sets the email publish error message.
        /// </summary>
        /// <value>
        /// The email publish error message.
        /// </value>
        public string? EmailPublishErrorMessage { get; set; }
    }
}
