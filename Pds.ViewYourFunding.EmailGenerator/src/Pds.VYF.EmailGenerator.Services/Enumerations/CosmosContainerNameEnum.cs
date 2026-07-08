// <copyright file="CosmosContainerNameEnum.cs" company="Department for Education - Skill Funding Services">
// Copyright (c) Department for Education - Skill Funding Services. All rights reserved.
// </copyright>

using System.Runtime.Serialization;

namespace Pds.VYF.EmailGenerator.Services.Enumerations
{
    /// <summary>
    /// Cosmos Container Name Enum.
    /// </summary>
    public enum CosmosContainerNameEnum
    {
        /// <summary>
        /// Funding.
        /// </summary>
        [EnumMember(Value = "Funding")]
        Funding = 0,

        /// <summary>
        /// Provider Funding.
        /// </summary>
        [EnumMember(Value = "Provider Funding")]
        ProviderFunding = 1,

        /// <summary>
        /// Audit.
        /// </summary>
        [EnumMember(Value = "Audit")]
        Audit = 2,
    }
}
