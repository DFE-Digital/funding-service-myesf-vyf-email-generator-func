// <copyright file="EmailPublishStatusEnum.cs" company="Department for Education - Skill Funding Services">
// Copyright (c) Department for Education - Skill Funding Services. All rights reserved.
// </copyright>

namespace Pds.VYF.EmailGenerator.Services.Enumerations
{
    /// <summary>
    /// The Email Publish Status Enum.
    /// </summary>
    public enum EmailPublishStatusEnum
    {
        /// <summary>
        /// The initial entry.
        /// </summary>
        InitialEntry,

        /// <summary>
        /// The email skipped.
        /// </summary>
        EmailSkipped,

        /// <summary>
        /// The email published.
        /// </summary>
        EmailPublished,

        /// <summary>
        /// The email failed to sent.
        /// </summary>
        EmailFailedToPublish,
    }
}
