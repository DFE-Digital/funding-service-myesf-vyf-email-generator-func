// <copyright file="FundingPeriodIdHelper.cs" company="Department for Education - Skill Funding Services">
// Copyright (c) Department for Education - Skill Funding Services. All rights reserved.
// </copyright>

namespace Pds.VYF.EmailGenerator.Services.Helpers
{
    /// <summary>
    /// The class for FundingPeriodIdHelper.
    /// </summary>
    public static class FundingPeriodIdHelper
    {
        /// <summary>
        /// Starts the year.
        /// </summary>
        /// <param name="fundingPeriodId">The funding period identifier.</param>
        /// <returns>Start Year in string Type.</returns>
        /// <exception cref="System.ArgumentException">Funding Period Id is in invalid format.</exception>
        public static string GetStartYear(this string fundingPeriodId) => GetYear(fundingPeriodId, 3);

        /// <summary>
        /// Ends the year.
        /// </summary>
        /// <param name="fundingPeriodId">The funding period identifier.</param>
        /// <returns>End Year in string Type.</returns>
        /// <exception cref="System.ArgumentException">Funding Period Id is in invalid format.</exception>
        public static string GetEndYear(this string fundingPeriodId) => GetYear(fundingPeriodId, 5);

        /// <summary>
        /// Gets the year.
        /// </summary>
        /// <param name="fundingPeriodId">The funding period identifier.</param>
        /// <param name="startIndex">The start index.</param>
        /// <returns>Year from the given start index with length of 2.</returns>
        /// <exception cref="System.ArgumentException">Funding Period Id is in invalid format.</exception>
        private static string GetYear(string fundingPeriodId, int startIndex)
        {
            if (int.TryParse(fundingPeriodId.Substring(startIndex, 2), out int intYear))
            {
                return (2000 + intYear).ToString();
            }

            throw new ArgumentException("Funding Period Id is in invalid format.");
        }
    }
}
