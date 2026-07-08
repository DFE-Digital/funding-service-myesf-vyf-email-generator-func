// <copyright file="FundingStreamCodeHelper.cs" company="Department for Education - Skill Funding Services">
// Copyright (c) Department for Education - Skill Funding Services. All rights reserved.
// </copyright>

namespace Pds.VYF.EmailGenerator.Services.Helpers
{
    /// <summary>
    /// The helper class for FundingStreamCodeHelper.
    /// </summary>
    public static class FundingStreamCodeHelper
    {
        /// <summary>
        /// Converts to fundingstreamname.
        /// </summary>
        /// <param name="fundingStreamCode">The funding stream code.</param>
        /// <returns>Funding Stream Name.</returns>
        public static string ToFundingStreamName(this string fundingStreamCode)
        {
            return fundingStreamCode.ToUpper() switch
            {
                "GAG" => "General Annual Grant",
                _ => throw new NotImplementedException(fundingStreamCode),
            };
        }

        /// <summary>
        /// Converts to fundingstreamnameforchildurl.
        /// </summary>
        /// <param name="fundingStreamCode">The funding stream code.</param>
        /// <returns>Funding Stream name for Child UI path.</returns>
        public static string ToFundingStreamNameForChildUrl(this string fundingStreamCode)
        {
            return fundingStreamCode.ToUpper() switch
            {
                "GAG" => "general-annual-grant",
                _ => throw new NotImplementedException(fundingStreamCode),
            };
        }
    }
}
