// <copyright file="TransformerResponse.cs" company="Department for Education - Skill Funding Services">
// Copyright (c) Department for Education - Skill Funding Services. All rights reserved.
// </copyright>

using Pds.VYF.EmailGenerator.Services.Models.AzureTableModels;

namespace Pds.VYF.EmailGenerator.Services.Models.Responses
{
    /// <summary>
    /// The class for TransformerResponse.
    /// </summary>
    /// <typeparam name="TAuditTableModel">>Any Audit Table Model.</typeparam>
    public class TransformerResponse<TAuditTableModel>
        where TAuditTableModel : BaseAuditTableModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransformerResponse{A}"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="emailToBeSentCount">The email to be sent count.</param>
        /// <param name="emailSkippedCount">The email skipped count.</param>
        public TransformerResponse(List<TAuditTableModel> value, int emailToBeSentCount, int emailSkippedCount)
        {
            this.Value = value;
            this.EmailToBeSentCount = emailToBeSentCount;
            this.EmailSkippedCount = emailSkippedCount;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public List<TAuditTableModel> Value { get; private set; }

        /// <summary>
        /// Gets the email to be sent count.
        /// </summary>
        /// <value>
        /// The email to be sent count.
        /// </value>
        public int EmailToBeSentCount { get; private set; }

        /// <summary>
        /// Gets the email skipped count.
        /// </summary>
        /// <value>
        /// The email skipped count.
        /// </value>
        public int EmailSkippedCount { get; }
    }
}
