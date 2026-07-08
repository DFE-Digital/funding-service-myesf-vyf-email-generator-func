// <copyright file="IAuditAndControlServices.cs" company="Department for Education - Skill Funding Services">
// Copyright (c) Department for Education - Skill Funding Services. All rights reserved.
// </copyright>

using Pds.VYF.EmailGenerator.Services.Enumerations;
using Pds.VYF.EmailGenerator.Services.Models.AzureTableModels;
using Pds.VYF.EmailGenerator.Services.Models.Requests;

namespace Pds.VYF.EmailGenerator.Services.Abstract.Controllers
{
    /// <summary>
    /// Interface for IAuditAndControlServices.
    /// </summary>
    public interface IAuditAndControlServices
    {
        // Control related Methods

        /// <summary>
        /// Gets the last status change date.
        /// </summary>
        /// <param name="processRequest">The process request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<string> GetLastStatusChangeDate(ProcessRequest processRequest, CancellationToken cancellationToken);

        /// <summary>
        /// Upserts the control entry asynchronous.
        /// </summary>
        /// <param name="processRequest">The process request.</param>
        /// <param name="statusChangeDate">The status change date.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task UpsertControlEntryAsync(ProcessRequest processRequest, string statusChangeDate, CancellationToken cancellationToken);

        // Audit related Methods

        /// <summary>
        /// Upserts the audit entry asynchronous.
        /// </summary>
        /// <param name="processRequest">The process request.</param>
        /// <param name="auditTableName">Name of the audit table.</param>
        /// <param name="auditEntity">The audit entity.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<int> UpsertAuditEntryAsync(ProcessRequest processRequest, string auditTableName, BaseAuditTableModel auditEntity, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the unsend audit entries asynchronous.
        /// </summary>
        /// <typeparam name="TAuditTableModel">The type of the audit table model.</typeparam>
        /// <param name="processRequest">The process request.</param>
        /// <param name="auditTableName">Name of the audit table.</param>
        /// <param name="emailPublishStatusToBeQueried">The email publish status to be queried.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<List<TAuditTableModel>> GetUnsendAuditEntriesAsync<TAuditTableModel>(ProcessRequest processRequest, string auditTableName, EmailPublishStatusEnum emailPublishStatusToBeQueried, CancellationToken cancellationToken)
            where TAuditTableModel : BaseAuditTableModel;

        /// <summary>
        /// Initiates the audit asynchronous.
        /// </summary>
        /// <typeparam name="TAuditTableModel">The type of the audit table model.</typeparam>
        /// <param name="processRequest">The process request.</param>
        /// <param name="auditEntities">The audit entities.</param>
        /// <param name="auditTableName">Name of the audit table.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task InitiateAuditAsync<TAuditTableModel>(ProcessRequest processRequest, List<TAuditTableModel> auditEntities, string auditTableName, CancellationToken cancellationToken)
            where TAuditTableModel : BaseAuditTableModel;
    }
}