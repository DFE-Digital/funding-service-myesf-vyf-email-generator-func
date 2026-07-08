// <copyright file="ICosmosContainerServices.cs" company="Department for Education - Skill Funding Services">
// Copyright (c) Department for Education - Skill Funding Services. All rights reserved.
// </copyright>

using Pds.VYF.EmailGenerator.Services.Enumerations;
using Pds.VYF.EmailGenerator.Services.Models.Responses;

namespace Pds.VYF.EmailGenerator.Services.Abstract.InfraServices
{
    /// <summary>
    /// Interface for ICosmosContainerServices.
    /// </summary>
    public interface ICosmosContainerServices
    {
        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <typeparam name="T">Any type.</typeparam>
        /// <param name="cosmosContainerNameEnum">The cosmos container name enum.</param>
        /// <param name="sqlQuery">The SQL query.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<CosmosQueryResponse<T>> GetAsync<T>(CosmosContainerNameEnum cosmosContainerNameEnum, string sqlQuery, CancellationToken cancellationToken);
    }
}
