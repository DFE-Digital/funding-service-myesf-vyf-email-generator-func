// <copyright file="IAzureTableServices.cs" company="Department for Education - Skill Funding Services">
// Copyright (c) Department for Education - Skill Funding Services. All rights reserved.
// </copyright>

using System.Linq.Expressions;
using Azure.Data.Tables;
using Pds.VYF.EmailGenerator.Services.Models.AzureTableModels;

namespace Pds.VYF.EmailGenerator.Services.Abstract.InfraServices
{
    /// <summary>
    /// Interface for IAzureTableServices.
    /// </summary>
    public interface IAzureTableServices
    {
        /// <summary>
        /// Queries the specified table name.
        /// </summary>
        /// <typeparam name="T">Any type which override <see cref="BaseTableEntity"/>.</typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="select">The select.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<List<T>> Query<T>(string tableName, Expression<Func<T, bool>> filter, IEnumerable<string>? select = null, CancellationToken cancellationToken = default)
        where T : BaseTableEntity;

        /// <summary>
        /// Upserts the entity asynchronous.
        /// </summary>
        /// <typeparam name="T">Any type which override <see cref="BaseTableEntity"/>.</typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Completed task.</returns>
        Task<int> UpsertEntityAsync<T>(string tableName, T entity, CancellationToken cancellationToken = default)
            where T : ITableEntity;

        /// <summary>
        /// Upserts the entities asynchronous.
        /// </summary>
        /// <typeparam name="T">Any type which override <see cref="BaseTableEntity"/>.</typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="entities">The entities.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Completed task.</returns>
        Task<int> UpsertEntitiesAsync<T>(string tableName, IEnumerable<T> entities, CancellationToken cancellationToken = default)
            where T : ITableEntity;
    }
}