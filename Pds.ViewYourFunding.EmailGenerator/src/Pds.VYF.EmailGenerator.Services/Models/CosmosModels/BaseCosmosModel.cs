// <copyright file="BaseCosmosModel.cs" company="Department for Education - Skill Funding Services">
// Copyright (c) Department for Education - Skill Funding Services. All rights reserved.
// </copyright>

namespace Pds.VYF.EmailGenerator.Services.Models.CosmosModels
{
    /// <summary>
    /// A class for BaseCosmosModel.
    /// </summary>
    public class BaseCosmosModel
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id { get; set; } = default!;

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
        public string StatusChangedDate { get; set; } = default!;
    }
}
