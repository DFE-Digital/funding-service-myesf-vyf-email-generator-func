// <copyright file="ParentAuditTableModel.cs" company="Department for Education - Skill Funding Services">
// Copyright (c) Department for Education - Skill Funding Services. All rights reserved.
// </copyright>

using Pds.VYF.EmailGenerator.Services.Enumerations;

namespace Pds.VYF.EmailGenerator.Services.Models.AzureTableModels
{
    /// <summary>
    /// A class for ParentAuditTableModel.
    /// </summary>
    /// <seealso cref="Pds.VYF.EmailGenerator.Services.Models.AzureTableModels.BaseAuditTableModel" />
    public class ParentAuditTableModel : BaseAuditTableModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParentAuditTableModel"/> class.
        /// </summary>
        public ParentAuditTableModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParentAuditTableModel" /> class.
        /// </summary>
        /// <param name="fundingStreamCode">The funding stream code.</param>
        /// <param name="fundingPeriodId">The funding period identifier.</param>
        /// <param name="ukprn">The ukprn.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="statusChangeDate">The status change date.</param>
        /// <param name="typeOfFunding">The type of funding.</param>
        /// <param name="organizationName">Name of the organization.</param>
        /// <param name="newProviderFundingCount">The new provider funding count.</param>
        /// <param name="updatedProviderFundingCount">The updated provider funding count.</param>
        /// <param name="providersWithNewFunding">The providers with new funding.</param>
        /// <param name="providersWithUpdatedFunding">The providers with updated funding.</param>
        public ParentAuditTableModel(
                                string fundingStreamCode,
                                string fundingPeriodId,
                                string ukprn,
                                string id,
                                string statusChangeDate,
                                string organizationName,
                                int newProviderFundingCount,
                                int updatedProviderFundingCount,
                                string providersWithNewFunding,
                                string providersWithUpdatedFunding)
            : base(fundingStreamCode, fundingPeriodId, ukprn, id, statusChangeDate)
        {
            this.OrganizationName = organizationName;
            this.NewProviderFundingCount = newProviderFundingCount;
            this.UpdatedProviderFundingCount = updatedProviderFundingCount;
            this.ProvidersWithNewFunding = providersWithNewFunding;
            this.ProvidersWithUpdatedFunding = providersWithUpdatedFunding;
        }

        /// <summary>
        /// Gets the type of the email.
        /// </summary>
        /// <value>
        /// The type of the email.
        /// </value>
        public override EmailTypesEnum EmailType => EmailTypesEnum.ForParents;

        /// <summary>
        /// Gets or sets the name of the organization.
        /// </summary>
        /// <value>
        /// The name of the organization.
        /// </value>
        public string OrganizationName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the new provider funding count.
        /// </summary>
        /// <value>
        /// The new provider funding count.
        /// </value>
        public int NewProviderFundingCount { get; set; }

        /// <summary>
        /// Gets or sets the updated provider funding count.
        /// </summary>
        /// <value>
        /// The updated provider funding count.
        /// </value>
        public int UpdatedProviderFundingCount { get; set; }

        /// <summary>
        /// Gets or sets the providers with new funding.
        /// </summary>
        /// <value>
        /// The providers with new funding.
        /// </value>
        public string? ProvidersWithNewFunding { get; set; }

        /// <summary>
        /// Gets or sets the providers with updated funding.
        /// </summary>
        /// <value>
        /// The providers with updated funding.
        /// </value>
        public string? ProvidersWithUpdatedFunding { get; set; }
    }
}
