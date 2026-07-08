// <copyright file="IEmailPublisher.cs" company="Department for Education - Skill Funding Services">
// Copyright (c) Department for Education - Skill Funding Services. All rights reserved.
// </copyright>

using Pds.VYF.EmailGenerator.Services.Models.AzureTableModels;
using Pds.VYF.EmailGenerator.Services.Models.Requests;

namespace Pds.VYF.EmailGenerator.Services.Abstract.Processors
{
    /// <summary>
    /// Interface for IEmailPublisher.
    /// </summary>
    public interface IEmailPublisher
    {
        Task<IList<string>> GetEmailAddresses(string ukprn);

        /// <summary>
        /// Publishes the asynchronous.
        /// </summary>
        /// <typeparam name="TAuditTableModel">Any class inherits from <see cref="BaseAuditTableModel"/>.</typeparam>
        /// <param name="processRequest">The process request.</param>
        /// <param name="auditEntity">The audit entity.</param>
        /// <param name="messageTypeFunc">The message type function.</param>
        /// <param name="personalisationFunc">The personalisation function.</param>
        /// <param name="auditTableName">Name of the audit table.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<int> PublishAsync<TAuditTableModel>(
                        ProcessRequest processRequest,
                        IEnumerable<TAuditTableModel> auditEntity,
                        Func<TAuditTableModel, string> messageTypeFunc,
                        Func<TAuditTableModel, IDictionary<string, object?>> personalisationFunc,
                        string auditTableName,
                        CancellationToken cancellationToken)
            where TAuditTableModel : BaseAuditTableModel;
    }
}
