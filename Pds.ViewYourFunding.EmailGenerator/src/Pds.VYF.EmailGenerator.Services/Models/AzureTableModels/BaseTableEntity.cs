// <copyright file="BaseTableEntity.cs" company="Department for Education - Skill Funding Services">
// Copyright (c) Department for Education - Skill Funding Services. All rights reserved.
// </copyright>

using Azure;

namespace Pds.VYF.EmailGenerator.Services.Models.AzureTableModels
{
    /// <summary>
    /// A class for BaseTableEntity.
    /// </summary>
    /// <seealso cref="Azure.Data.Tables.ITableEntity" />
    public abstract class BaseTableEntity : Azure.Data.Tables.ITableEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseTableEntity"/> class.
        /// </summary>
        /// <param name="partitionKey">The partition key.</param>
        /// <param name="rowKey">The row key.</param>
        protected BaseTableEntity(string partitionKey, string rowKey)
        {
            this.PartitionKey = partitionKey;
            this.RowKey = rowKey;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseTableEntity"/> class.
        /// </summary>
        protected BaseTableEntity()
        {
        }

        /// <summary>
        /// The partition key is a unique identifier for the partition within a given table and forms the first part of an entity's primary key.
        /// </summary>
        /// <value>
        /// A string containing the partition key for the entity.
        /// </value>
        public string PartitionKey { get; set; } = default!;

        /// <summary>
        /// The row key is a unique identifier for an entity within a given partition. Together the <see cref="P:Azure.Data.Tables.ITableEntity.PartitionKey" /> and RowKey uniquely identify every entity within a table.
        /// </summary>
        /// <value>
        /// A string containing the row key for the entity.
        /// </value>
        public string RowKey { get; set; } = default!;

        /// <summary>
        /// The Timestamp property is a DateTime value that is maintained on the server side to record the time an entity was last modified.
        /// The Table service uses the Timestamp property internally to provide optimistic concurrency. The value of Timestamp is a monotonically increasing value,
        /// meaning that each time the entity is modified, the value of Timestamp increases for that entity.
        /// This property should not be set on insert or update operations (the value will be ignored).
        /// </summary>
        /// <value>
        /// A <see cref="T:System.DateTimeOffset" /> containing the timestamp of the entity.
        /// </value>
        public DateTimeOffset? Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the entity's ETag.
        /// </summary>
        /// <value>
        /// A string containing the ETag value for the entity.
        /// </value>
        public ETag ETag { get; set; }
    }
}
