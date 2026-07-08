// <copyright file="ParentCosmosModel.cs" company="Department for Education - Skill Funding Services">
// Copyright (c) Department for Education - Skill Funding Services. All rights reserved.
// </copyright>

namespace Pds.VYF.EmailGenerator.Services.Models.CosmosModels
{
    /// <summary>
    /// A class for ParentModel.
    /// </summary>
    /// <seealso cref="Pds.VYF.EmailGenerator.Services.Models.CosmosModels.BaseCosmosModel" />
    public class ParentCosmosModel : BaseCosmosModel
    {
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
