// <copyright file="ControlTableModel.cs" company="Department for Education - Skill Funding Services">
// Copyright (c) Department for Education - Skill Funding Services. All rights reserved.
// </copyright>

namespace Pds.VYF.EmailGenerator.Services.Models.AzureTableModels
{
    /// <summary>
    /// A class for ControlTableModel.
    /// </summary>
    /// <seealso cref="Pds.VYF.EmailGenerator.Services.Models.AzureTableModels.BaseTableEntity" />
    public class ControlTableModel : BaseTableEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ControlTableModel"/> class.
        /// </summary>
        public ControlTableModel()
            : base(string.Empty, string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ControlTableModel"/> class.
        /// </summary>
        /// <param name="partitionKey">The partition key.</param>
        /// <param name="rowKey">The row key.</param>
        public ControlTableModel(string partitionKey, string rowKey)
            : base(partitionKey, rowKey)
        {
        }

        /// <summary>
        /// Gets or sets the status changed date.
        /// </summary>
        /// <value>
        /// The status changed date.
        /// </value>
        public string? StatusChangedDate { get; set; }
    }
}
