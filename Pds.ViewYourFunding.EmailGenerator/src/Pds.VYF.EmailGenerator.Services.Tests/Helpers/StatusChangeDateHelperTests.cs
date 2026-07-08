// <copyright file="StatusChangeDateHelperTests.cs" company="Department for Education - Skill Funding Services">
// Copyright (c) Department for Education - Skill Funding Services. All rights reserved.
// </copyright>

using FluentAssertions;

namespace Pds.VYF.EmailGenerator.Services.Helpers.Tests
{
    /// <summary>
    /// The class for StatusChangeDateHelper Tests.
    /// </summary>
    [TestClass]
    public class StatusChangeDateHelperTests
    {
        /// <summary>
        /// Formats the status change date for child URL valid input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="expectedResult">The expected result.</param>
        [TestMethod]
        [DataRow("2023-01-05", "05-01-2023")]
        [DataRow("2023-11-05", "05-11-2023")]
        [DataRow("2023-01-15", "15-01-2023")]
        public void FormatStatusChangeDateForChildUrl_ValidInput(string input, string expectedResult)
        {
            // Act
            string actualResult = input.FormatStatusChangeDateForChildUrl();

            // Assert
            actualResult.Should().Be(expectedResult);
        }

        /// <summary>
        /// Formats the status change date for child URL empty input should throw argument exception.
        /// </summary>
        [TestMethod]
        public void FormatStatusChangeDateForChildUrl_EmptyInput_ShouldThrowArgumentException()
        {
            // Arrange
            string statusChangeDate = string.Empty;

            // Act
            Action act = () => statusChangeDate.FormatStatusChangeDateForChildUrl();

            // Assert
            act.Should().Throw<ArgumentException>().WithMessage("*The value cannot be an empty string or composed entirely of whitespace.*");
        }

        /// <summary>
        /// Formats the status change date for child URL null input should throw argument null exception.
        /// </summary>
        [TestMethod]
        public void FormatStatusChangeDateForChildUrl_NullInput_ShouldThrowArgumentNullException()
        {
            // Arrange
            string? statusChangeDate = null;

            // Act
            Action act = () => statusChangeDate.FormatStatusChangeDateForChildUrl();

            // Assert
            act.Should().Throw<ArgumentException>().WithMessage("*Value cannot be null*");
        }
    }
}