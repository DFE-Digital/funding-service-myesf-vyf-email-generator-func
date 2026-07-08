// <copyright file="FundingPeriodIdHelperTests.cs" company="Department for Education - Skill Funding Services">
// Copyright (c) Department for Education - Skill Funding Services. All rights reserved.
// </copyright>

using FluentAssertions;

namespace Pds.VYF.EmailGenerator.Services.Helpers.Tests
{
    /// <summary>
    /// The Test class for FundingPeriodIdHelper.
    /// </summary>
    [TestClass]
    public class FundingPeriodIdHelperTests
    {
        /// <summary>
        /// Starts the year test.
        /// </summary>
        /// <param name="fundingPeriodId">The funding period identifier.</param>
        /// <param name="expectedResult">The expected result.</param>
        [TestMethod]
        [DataRow("AY-2324", "2023")]
        [DataRow("AY-1920", "2019")]
        [DataRow("AY-2526", "2025")]
        public void StartYearTest(string fundingPeriodId, string expectedResult)
        {
            // Act
            var actualResult = fundingPeriodId.GetStartYear();

            // Assert
            actualResult.Should().Be(expectedResult);
        }

        /// <summary>
        /// Ends the year test.
        /// </summary>
        /// <param name="fundingPeriodId">The funding period identifier.</param>
        /// <param name="expectedResult">The expected result.</param>
        [TestMethod]
        [DataRow("AY-2324", "2024")]
        [DataRow("AY-1920", "2020")]
        [DataRow("AY-2526", "2026")]
        public void EndYearTest(string fundingPeriodId, string expectedResult)
        {
            // Act
            var actualResult = fundingPeriodId.GetEndYear();

            // Assert
            actualResult.Should().Be(expectedResult);
        }
    }
}