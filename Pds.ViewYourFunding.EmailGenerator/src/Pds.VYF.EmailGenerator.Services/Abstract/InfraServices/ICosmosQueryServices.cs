// <copyright file="ICosmosQueryServices.cs" company="Department for Education - Skill Funding Services">
// Copyright (c) Department for Education - Skill Funding Services. All rights reserved.
// </copyright>

using Pds.VYF.EmailGenerator.Services.Models.Requests;

namespace Pds.VYF.EmailGenerator.Services.Abstract.InfraServices
{
    /// <summary>
    /// Interface for ICosmosQueryServices.
    /// </summary>
    public interface ICosmosQueryServices
    {
        /// <summary>
        /// Gets the child query.
        /// </summary>
        /// <param name="processRequest">The process request.</param>
        /// <param name="statusChangeDate">The status change date.</param>
        /// <returns>
        /// string.
        /// </returns>
        string GetChildQuery(ProcessRequest processRequest, string statusChangeDate);

        /// <summary>
        /// Gets the parent query.
        /// </summary>
        /// <param name="processRequest">The process request.</param>
        /// <param name="statusChangeDate">The status change date.</param>
        /// <returns>
        /// string.
        /// </returns>
        string GetParentQuery(ProcessRequest processRequest, string statusChangeDate);

        /// <summary>
        /// Gets the child with parent identifier query.
        /// </summary>
        /// <param name="processRequest">The process request.</param>
        /// <param name="statusChangeDate">The status change date.</param>
        /// <param name="organizationId">The organization identifier.</param>
        /// <returns>
        /// string.
        /// </returns>
        string GetChildWithParentIdQuery(ProcessRequest processRequest, string statusChangeDate, IEnumerable<string> organizationId);

        /// <summary>
        /// Gets the last feed reader audit query.
        /// </summary>
        /// <returns>string.</returns>
        string GetLastFeedReaderAuditQuery();
    }
}
