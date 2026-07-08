// <copyright file="StringExtensionsTests.cs" company="Department for Education - Skill Funding Services">
// Copyright (c) Department for Education - Skill Funding Services. All rights reserved.
// </copyright>

using FluentAssertions;
using Pds.VYF.EmailGenerator.Services.Extensions;

namespace Pds.VYF.EmailGenerator.Services.Tests.Extensions
{
    /// <summary>
    /// The class for StringExtensions Tests.
    /// </summary>
    [TestClass]
    public class StringExtensionsTests
    {
        /// <summary>
        /// Adds the quote in each value success.
        /// </summary>
        /// <param name="testCaseName">Name of the test case.</param>
        /// <param name="delimiter">The delimiter.</param>
        /// <param name="isSingleQuote">if set to <c>true</c> [is single quote].</param>
        /// <param name="input">The input.</param>
        /// <param name="expectedResult">The expected result.</param>
        [DataRow("Single Quote", ",", true, "apple,banana,cherry", "'apple','banana','cherry'")]
        [DataRow("Quote in Single Value", ",", true, "apple", "'apple'")]
        [DataRow("Empty String", ",", true, "", "")]
        [DataRow("Double Quote", ",", false, "apple,banana,cherry", "\"apple\",\"banana\",\"cherry\"")]
        [DataRow("Null value", ",", false, null, "")]
        [TestMethod]
        public void AddQuoteInEachValue_Success(string testCaseName, string delimiter, bool isSingleQuote, string input, string expectedResult)
        {
            // Act
            string actualResult = input.AddQuoteInEachValue(delimiter, isSingleQuote);

            // Assert
            actualResult.Should().Be(expectedResult, testCaseName);
        }

        /// <summary>
        /// Firsts the character to upper success.
        /// </summary>
        /// <param name="testCaseName">Name of the test case.</param>
        /// <param name="input">The input.</param>
        /// <param name="expectedResult">The expected result.</param>
        [DataRow("Valid Input", "hello", "Hello")]
        [DataRow("Empty Input", "", "")]
        [DataRow("Null Input", null, "")]
        [DataRow("White space Input", "   ", "")]
        [TestMethod]
        public void FirstCharToUpper_Success(string testCaseName, string input, string expectedResult)
        {
            // Act
            string actualResult = input.FirstCharToUpper();

            // Assert
            actualResult.Should().Be(expectedResult, testCaseName);
        }

        /// <summary>
        /// Adds the spaces to sentence success.
        /// </summary>
        /// <param name="testCaseName">Name of the test case.</param>
        /// <param name="preserveAcronyms">The preserve acronyms.</param>
        /// <param name="input">The input.</param>
        /// <param name="expectedResult">The expected result.</param>
        [DataRow("Valid Input", null, "ThisIsATest", "This Is A Test")]
        [DataRow("Preserve Acronyms", true, "ThisIsATestABC", "This Is A Test ABC")]
        [DataRow("Do Not Preserve Acronyms", false, "ThisIsATestABC", "This Is A Test A B C")]
        [DataRow("Sentence with space", false, "Total Extracted Items Count", "Total Extracted Items Count")]
        [DataRow("Empty Input", null, "", "")]
        [DataRow("Null Input", null, null, "")]
        [DataRow("White Space Input", null, "   ", "")]
        [TestMethod]
        public void AddSpacesToSentence_Success(string testCaseName, bool? preserveAcronyms, string input, string expectedResult)
        {
            // Act
            string actualResult = preserveAcronyms == null ? input.AddSpacesToSentence() : input.AddSpacesToSentence(preserveAcronyms.Value);

            // Assert
            actualResult.Should().Be(expectedResult, testCaseName);
        }
    }
}