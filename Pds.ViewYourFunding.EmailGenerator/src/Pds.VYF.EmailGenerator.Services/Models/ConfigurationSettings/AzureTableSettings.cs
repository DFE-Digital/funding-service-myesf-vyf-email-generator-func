// <copyright file="AzureTableSettings.cs" company="Department for Education - Skill Funding Services">
// Copyright (c) Department for Education - Skill Funding Services. All rights reserved.
// </copyright>

using System.ComponentModel.DataAnnotations;

namespace Pds.VYF.EmailGenerator.Services.Models.ConfigurationSettings
{
    /// <summary>
    /// Azure Table Settings.
    /// </summary>
    public class AzureTableSettings
    {
        /// <summary>
        /// The option name.
        /// </summary>
        public const string OptionName = "AzureStorageConfiguration";

        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>
        /// The connection string.
        /// </value>
        [Required(ErrorMessage = $"{OptionName}:{nameof(ConnectionString)} is missing.")]
        public string ConnectionString { get; set; } = default!;

        /// <summary>
        /// Gets or sets the name of the control table.
        /// </summary>
        /// <value>
        /// The name of the control table.
        /// </value>
        [Required(ErrorMessage = $"{OptionName}:{nameof(ControlTableName)} is missing.")]
        public string ControlTableName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the name of the single provider audit table.
        /// </summary>
        /// <value>
        /// The name of the single provider audit table.
        /// </value>
        [Required(ErrorMessage = $"{OptionName}:{nameof(ChildAuditTableName)} is missing.")]
        public string ChildAuditTableName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the name of the multi academy audit table.
        /// </summary>
        /// <value>
        /// The name of the multi academy audit table.
        /// </value>
        [Required(ErrorMessage = $"{OptionName}:{nameof(ParentAuditTableName)} is missing.")]
        public string ParentAuditTableName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the notify service template table.
        /// </summary>
        /// <value>
        /// The notify service template table.
        /// </value>
        [Required(ErrorMessage = $"{OptionName}:{nameof(NotifyServiceTemplateTable)} is missing.")]
        public string NotifyServiceTemplateTable { get; set; } = default!;

        /// <summary>
        /// Gets or sets the maximum per page.
        /// </summary>
        /// <value>
        /// The maximum per page.
        /// </value>
        [Range(50, 1000, ErrorMessage = $"{OptionName}:{nameof(MaxPerPage)} should be within the range of 50 to 1000.")]
        public int MaxPerPage { get; set; } = 100;
    }
}