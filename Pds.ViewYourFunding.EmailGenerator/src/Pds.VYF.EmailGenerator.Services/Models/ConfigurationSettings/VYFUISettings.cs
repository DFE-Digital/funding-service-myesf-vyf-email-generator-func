// <copyright file="VYFUISettings.cs" company="Department for Education - Skill Funding Services">
// Copyright (c) Department for Education - Skill Funding Services. All rights reserved.
// </copyright>

using System.ComponentModel.DataAnnotations;

namespace Pds.VYF.EmailGenerator.Services.Models.ConfigurationSettings
{
    /// <summary>
    /// The class for VYFUISettings.
    /// </summary>
    public class VYFUISettings
    {
        /// <summary>
        /// The option name.
        /// </summary>
        public const string OptionName = "VYFUIApiConfiguration";

        /// <summary>
        /// Gets or sets the base URI.
        /// </summary>
        /// <value>
        /// The base URI.
        /// </value>
        [Required(ErrorMessage = $"{OptionName}:{nameof(BaseUri)} is missing.")]
        public string BaseUri { get; set; } = default!;

        /// <summary>
        /// Gets or sets the API key.
        /// </summary>
        /// <value>
        /// The API key.
        /// </value>
        [Required(ErrorMessage = $"{OptionName}:{nameof(ApiKey)} is missing.")]
        public string ApiKey { get; set; } = default!;

        /// <summary>
        /// Gets or sets the email enabled funding stream and periods endpoint URI.
        /// </summary>
        /// <value>
        /// The email enabled funding stream and periods endpoint URI.
        /// </value>
        [Required(ErrorMessage = $"{OptionName}:{nameof(EmailEnabledFundingStreamAndPeriodsEndpointUri)} is missing.")]
        public string EmailEnabledFundingStreamAndPeriodsEndpointUri { get; set; } = default!;

        /// <summary>
        /// Gets or sets the latest funding stream published date endpoint URI.
        /// </summary>
        /// <value>
        /// The latest funding stream published date endpoint URI.
        /// </value>
        [Required(ErrorMessage = $"{OptionName}:{nameof(LatestFundingStreamPublishedDateEndpointUri)} is missing.")]
        public string LatestFundingStreamPublishedDateEndpointUri { get; set; } = default!;
    }
}
