// <copyright file="IBaseProcessor.cs" company="Department for Education - Skill Funding Services">
// Copyright (c) Department for Education - Skill Funding Services. All rights reserved.
// </copyright>

using Pds.VYF.EmailGenerator.Services.Models.Requests;

namespace Pds.VYF.EmailGenerator.Services.Abstract.Processors
{
    /// <summary>
    /// Interface for IBaseProcessors.
    /// </summary>
    public interface IBaseProcessor
    {
        /// <summary>
        /// Gets the name of the audit azure table.
        /// </summary>
        /// <value>
        /// The name of the audit azure table.
        /// </value>
        string AuditAzureTableName { get; }

        /// <summary>
        /// Processes the asynchronous.
        /// </summary>
        /// <param name="processRequest">The process request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ProcessAsync(ProcessRequest processRequest, CancellationToken cancellationToken);
    }
}
