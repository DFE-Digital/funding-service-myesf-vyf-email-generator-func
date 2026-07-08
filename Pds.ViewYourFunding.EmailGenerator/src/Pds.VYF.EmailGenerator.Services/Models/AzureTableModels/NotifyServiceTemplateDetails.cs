// <copyright file="NotifyServiceTemplateDetails.cs" company="Department for Education - Skill Funding Services">
// Copyright (c) Department for Education - Skill Funding Services. All rights reserved.
// </copyright>

namespace Pds.VYF.EmailGenerator.Services.Models.AzureTableModels
{
    /// <summary>
    /// A class for NotifyServiceTemplateDetails.
    /// </summary>
    /// <seealso cref="Pds.VYF.EmailGenerator.Services.Models.AzureTableModels.BaseTableEntity" />
    internal class NotifyServiceTemplateDetails : BaseTableEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyServiceTemplateDetails"/> class.
        /// </summary>
        public NotifyServiceTemplateDetails()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyServiceTemplateDetails"/> class.
        /// </summary>
        /// <param name="partitionKey">The partition key.</param>
        /// <param name="rowKey">The row key.</param>
        public NotifyServiceTemplateDetails(string partitionKey, string rowKey)
            : base(partitionKey, rowKey)
        {
        }

        /// <summary>
        /// Gets or sets the requesting service.
        /// </summary>
        /// <value>
        /// The requesting service.
        /// </value>
        public string? RequestingService { get; set; }

        /// <summary>
        /// Gets or sets the metadata for notify email.
        /// </summary>
        /// <value>
        /// The metadata.
        /// </value>
        public string? Metadata { get; set; }

        /// <summary>
        /// Gets or sets the template id for notify email.
        /// </summary>
        /// <value>
        /// The template identifier.
        /// </value>
        public string? TemplateId { get; set; }

        /// <summary>
        /// Gets or sets the NotifyApiKeySecretName for notify email.
        /// </summary>
        /// <value>
        /// The name of the notify API key secret.
        /// </value>
        public string? NotifyApiKeySecretName { get; set; }
    }
}