// <copyright file="StatusChangeDateHelper.cs" company="Department for Education - Skill Funding Services">
// Copyright (c) Department for Education - Skill Funding Services. All rights reserved.
// </copyright>

namespace Pds.VYF.EmailGenerator.Services.Helpers
{
    /// <summary>
    /// The Extension class for StatusChangeDateHelper.
    /// </summary>
    public static class StatusChangeDateHelper
    {
        /// <summary>
        /// Formats the status change date for child URL.
        /// </summary>
        /// <param name="statusChangeDate">The status change date.</param>
        /// <returns>Convert status change date to DD-MM-YYYY format.</returns>
        public static string FormatStatusChangeDateForChildUrl(this string? statusChangeDate)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(statusChangeDate);

            if (statusChangeDate.Length < 10)
            {
                throw new ArgumentException("Minimum length of the status change date should be 10.", nameof(statusChangeDate));
            }

            return statusChangeDate.Substring(8, 2) + "-" + statusChangeDate.Substring(5, 2) + "-" + statusChangeDate[..4];
        }
    }
}
