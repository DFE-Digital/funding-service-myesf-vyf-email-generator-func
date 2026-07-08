// <copyright file="AppSettings.cs" company="Department for Education - Skill Funding Services">
// Copyright (c) Department for Education - Skill Funding Services. All rights reserved.
// </copyright>

using System.ComponentModel.DataAnnotations;
using Pds.VYF.EmailGenerator.Services.Enumerations;

namespace Pds.VYF.EmailGenerator.Services.Models.ConfigurationSettings
{
    /// <summary>
    /// The Service Configuration.
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// The option name.
        /// </summary>
        public const string OptionName = "ServiceConfiguration";

        /// <summary>
        /// Gets or sets the email run mode.
        /// </summary>
        /// <value>
        /// The email run mode.
        /// </value>
        [Required(ErrorMessage = $"{OptionName}:{nameof(EmailRunMode)} is missing.")]
        public virtual EmailRunModeEnum EmailRunMode { get; set; } = EmailRunModeEnum.Test;

        /// <summary>
        /// Gets or sets the test email addresses.
        /// </summary>
        /// <value>
        /// The test email addresses.
        /// </value>
        public virtual string? TestEmailAddresses { get; set; } = default!;

        /// <summary>
        /// Gets or sets the test email addresses only to be published.
        /// </summary>
        /// <value>
        /// The test email addresses only to be published.
        /// </value>
        public virtual string? InternalEmailAddresses { get; set; } = default!;

        /// <summary>
        /// Gets or sets the funding filter variation reasons.
        /// </summary>
        /// <value>
        /// The funding filter variation reasons.
        /// </value>
        [Required(ErrorMessage = $"{OptionName}:{nameof(FundingFilterVariationReasons)} is missing.")]
        public virtual string FundingFilterVariationReasons { get; set; } = default!;

        /// <summary>
        /// Gets or sets the requesting service.
        /// </summary>
        /// <value>
        /// The requesting service.
        /// </value>
        [Required(ErrorMessage = $"{OptionName}:{nameof(RequestingService)} is missing.")]
        public virtual string RequestingService { get; set; } = default!;

        /// <summary>
        /// Gets or sets the name of the notify API key secret.
        /// </summary>
        /// <value>
        /// The name of the notify API key secret.
        /// </value>
        [Required(ErrorMessage = $"{OptionName}:{nameof(NotifyApiKeySecretName)} is missing.")]
        public virtual string NotifyApiKeySecretName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the email templates.
        /// </summary>
        /// <value>
        /// The email templates.
        /// </value>
        [Required(ErrorMessage = $"{OptionName}:{nameof(EmailTemplates)} is missing.")]
        public virtual EmailTemplatesModel EmailTemplates { get; set; } = default!;

        /// <summary>
        /// Gets or sets the size of the email published batch.
        /// </summary>
        /// <value>
        /// The size of the email published batch.
        /// </value>
        [Range(10, 500, ErrorMessage = $"{OptionName}:{nameof(EmailPublishedBatchSize)} should be within the range of 10 to 500.")]
        public virtual int EmailPublishedBatchSize { get; set; } = 100;

        /// <summary>
        /// Gets or sets the size of the parent search batch.
        /// </summary>
        /// <value>
        /// The size of the parent search batch.
        /// </value>
        [Range(5, 50, ErrorMessage = $"{OptionName}:{nameof(ParentSearchBatchSize)} should be within the range of 5 to 50.")]
        public virtual int ParentSearchBatchSize { get; set; } = 20;

        /// <summary>
        /// Gets or sets the UI base URI.
        /// </summary>
        /// <value>
        /// The UI base URI.
        /// </value>
        [Required(ErrorMessage = $"{OptionName}:{nameof(UIBaseUri)} is missing.")]
        public virtual string UIBaseUri { get; set; } = default!;

        /// <summary>
        /// Gets or sets the UI parent URL.
        /// </summary>
        /// <value>
        /// The UI parent URL.
        /// </value>
        [Required(ErrorMessage = $"{OptionName}:{nameof(UIParentUrl)} is missing.")]
        public virtual string UIParentUrl { get; set; } = default!;

        /// <summary>
        /// Gets or sets the UI child URL.
        /// </summary>
        /// <value>
        /// The UI child URL.
        /// </value>
        [Required(ErrorMessage = $"{OptionName}:{nameof(UIChildUrl)} is missing.")]
        public virtual string UIChildUrl { get; set; } = default!;
    }
}