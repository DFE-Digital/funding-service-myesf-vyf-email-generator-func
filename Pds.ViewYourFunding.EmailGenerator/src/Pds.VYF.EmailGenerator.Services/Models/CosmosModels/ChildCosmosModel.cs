// <copyright file="ChildCosmosModel.cs" company="Department for Education - Skill Funding Services">
// Copyright (c) Department for Education - Skill Funding Services. All rights reserved.
// </copyright>

using Pds.VYF.EmailGenerator.Services.Enumerations;

namespace Pds.VYF.EmailGenerator.Services.Models.CosmosModels
{
    /// <summary>
    /// A class for ChildModel.
    /// </summary>
    /// <seealso cref="Pds.VYF.EmailGenerator.Services.Models.CosmosModels.BaseCosmosModel" />
    public class ChildCosmosModel : BaseCosmosModel
    {
        /// <summary>
        /// Gets or sets the type of funding.
        /// </summary>
        /// <value>
        /// The type of funding.
        /// </value>
        public TypeOfFundingEnum TypeOfFunding { get; set; } = default!;

        /// <summary>
        /// Gets or sets the name of the provider.
        /// </summary>
        /// <value>
        /// The name of the provider.
        /// </value>
        public string ProviderName { get; set; } = default!;
    }
}
