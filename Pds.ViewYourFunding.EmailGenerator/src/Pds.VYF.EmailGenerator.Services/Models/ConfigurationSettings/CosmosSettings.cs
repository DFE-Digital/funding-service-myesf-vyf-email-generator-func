// <copyright file="CosmosSettings.cs" company="Department for Education - Skill Funding Services">
// Copyright (c) Department for Education - Skill Funding Services. All rights reserved.
// </copyright>

using Microsoft.Azure.Cosmos;
using System.ComponentModel.DataAnnotations;

namespace Pds.VYF.EmailGenerator.Services.Models.ConfigurationSettings
{
    /// <summary>
    /// The class for CosmosSettings.
    /// </summary>
    public class CosmosSettings
    {
        /// <summary>
        /// The option name.
        /// </summary>
        public const string OptionName = "CosmosDBConfiguration";

        /// <summary>
        /// Gets or sets the endpoint URI.
        /// </summary>
        /// <value>
        /// The endpoint URI.
        /// </value>
        [Required(ErrorMessage = $"{OptionName}:{nameof(AccountEndpoint)} is missing.")]
        public string AccountEndpoint { get; set; } = default!;

        /// <summary>
        /// Gets or sets the endpoint key.
        /// </summary>
        /// <value>
        /// The endpoint key.
        /// </value>
        [Required(ErrorMessage = $"{OptionName}:{nameof(AccountKey)} is missing.")]
        public string AccountKey { get; set; } = default!;

        /// <summary>
        /// Gets or sets the name of the database.
        /// </summary>
        /// <value>
        /// The name of the database.
        /// </value>
        [Required(ErrorMessage = $"{OptionName}:{nameof(Database)} is missing.")]
        public string Database { get; set; } = default!;

        /// <summary>
        /// Gets or sets the name of the funding group collection.
        /// </summary>
        /// <value>
        /// The name of the funding group collection.
        /// </value>
        [Required(ErrorMessage = $"{OptionName}:{nameof(FundingGroupCollectionName)} is missing.")]
        public string FundingGroupCollectionName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the name of the provider funding collection.
        /// </summary>
        /// <value>
        /// The name of the provider funding collection.
        /// </value>
        [Required(ErrorMessage = $"{OptionName}:{nameof(ProviderFundingCollectionName)} is missing.")]
        public string ProviderFundingCollectionName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the name of the audit collection.
        /// </summary>
        /// <value>
        /// The name of the audit collection.
        /// </value>
        [Required(ErrorMessage = $"{OptionName}:{nameof(AuditCollectionName)} is missing.")]
        public string AuditCollectionName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the maximum item count.
        /// </summary>
        /// <value>
        /// The maximum item count.
        /// </value>
        [Required(ErrorMessage = $"{OptionName}:{nameof(MaxItemCount)} is missing.")]
        public int MaxItemCount { get; set; } = default!;

        /// <summary>
        /// Gets or sets the connection mode.
        /// </summary>
        /// <value>
        /// The connection mode.
        /// </value>
        public ConnectionMode ConnectionMode { get; set; } = ConnectionMode.Direct;


        /// <summary>
        /// Gets or sets the high throughput.
        /// </summary>
        /// <value>
        /// The high throughput.
        /// </value>
        public int HighThroughput { get; set; } = 5000;

        /// <summary>
        /// Gets or sets the low throughput.
        /// </summary>
        /// <value>
        /// The low throughput.
        /// </value>
        public int LowThroughput { get; set; } = 400;
    }
}
