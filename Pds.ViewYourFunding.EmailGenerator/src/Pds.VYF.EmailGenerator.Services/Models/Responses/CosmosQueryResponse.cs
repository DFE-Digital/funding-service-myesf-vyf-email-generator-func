// <copyright file="CosmosQueryResponse.cs" company="Department for Education - Skill Funding Services">
// Copyright (c) Department for Education - Skill Funding Services. All rights reserved.
// </copyright>

namespace Pds.VYF.EmailGenerator.Services.Models.Responses
{
    /// <summary>
    /// The Cosmos Query Response.
    /// </summary>
    /// <typeparam name="T">Type Param.</typeparam>
    public class CosmosQueryResponse<T>
    {
        /// <summary>
        /// Gets the results.
        /// </summary>
        /// <value>
        /// The results.
        /// </value>
        public List<T> Results { get; private set; } = [];

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        public int Count => this.Results.Count;

        /// <summary>
        /// Gets or sets the time taken.
        /// </summary>
        /// <value>
        /// The time taken.
        /// </value>
        public TimeSpan TimeTaken { get; set; }

        /// <summary>
        /// Gets or sets the request charge.
        /// </summary>
        /// <value>
        /// The request charge.
        /// </value>
        public double RequestCharge { get; set; } = 0;
    }
}