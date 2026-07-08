// <copyright file="ChildAuditTableModel.cs" company="Department for Education - Skill Funding Services">
// Copyright (c) Department for Education - Skill Funding Services. All rights reserved.
// </copyright>

using Pds.VYF.EmailGenerator.Services.Enumerations;

namespace Pds.VYF.EmailGenerator.Services.Models.AzureTableModels
{
    /// <summary>
    /// A class for ChildAuditTableModel.
    /// </summary>
    /// <seealso cref="Pds.VYF.EmailGenerator.Services.Models.AzureTableModels.BaseAuditTableModel" />
    public class ChildAuditTableModel : BaseAuditTableModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChildAuditTableModel"/> class.
        /// </summary>
        public ChildAuditTableModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChildAuditTableModel"/> class.
        /// </summary>
        /// <param name="fundingStreamCode">The funding stream code.</param>
        /// <param name="fundingPeriodId">The funding period identifier.</param>
        /// <param name="ukprn">The ukprn.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="statusChangeDate">The status change date.</param>
        /// <param name="providerName">Name of the provider.</param>
        /// <param name="typeOfFunding">The type of funding.</param>
        public ChildAuditTableModel(
                                string fundingStreamCode,
                                string fundingPeriodId,
                                string ukprn,
                                string id,
                                string statusChangeDate,
                                string providerName,
                                TypeOfFundingEnum typeOfFunding)
            : base(fundingStreamCode, fundingPeriodId, ukprn, id, statusChangeDate)
        {
            this.TypeOfFunding = typeOfFunding;
            this.ProviderName = providerName;
        }

        /// <summary>
        /// Gets or sets the type of funding.
        /// </summary>
        /// <value>
        /// The type of funding.
        /// </value>
        public TypeOfFundingEnum TypeOfFunding { get; set; } = default!;

        /// <summary>
        /// Gets the type of the email.
        /// </summary>
        /// <value>
        /// The type of the email.
        /// </value>
        public override EmailTypesEnum EmailType => EmailTypesEnum.ForChildren;

        /// <summary>
        /// Gets or sets the name of the provider.
        /// </summary>
        /// <value>
        /// The name of the provider.
        /// </value>
        public string ProviderName { get; set; } = default!;
    }
}
