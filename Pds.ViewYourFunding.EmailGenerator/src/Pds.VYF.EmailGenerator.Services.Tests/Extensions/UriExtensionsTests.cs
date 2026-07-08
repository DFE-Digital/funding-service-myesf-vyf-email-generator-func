// <copyright file="UriExtensionsTests.cs" company="Department for Education - Skill Funding Services">
// Copyright (c) Department for Education - Skill Funding Services. All rights reserved.
// </copyright>

using FluentAssertions;
using Pds.VYF.EmailGenerator.Services.Extensions;

namespace Pds.VYF.EmailGenerator.Services.Tests.Extensions
{
    /// <summary>
    /// The class for UriExtensions Tests.
    /// </summary>
    [TestClass]
    public class UriExtensionsTests
    {
        /// <summary>
        /// Appends the valid paths.
        /// </summary>
        /// <param name="expectedResult">The expected result.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="paths">The paths.</param>
        [TestMethod]
        [DataRow("https://example.com/path1/path2/path3", "https://example.com", "path1", "path2", "path3")]
        [DataRow("https://example.com/path1/path2/path3", "https://example.com/", "path1", "path2", "path3")]
        [DataRow("https://example.com/path1/path2/path3", "https://example.com", "/path1", "path2/", "path3")]
        [DataRow("https://example.com/path1/path2/path3", "https://example.com/", "path1", "path2", "/path3/")]
        [DataRow("https://example.com/path1/path2/path3", "https://example.com/", "/path1/", "/path2/", "/path3/")]
        public void Append_ValidPaths(string expectedResult, string uri, params string[] paths)
        {
            // Arrange
            Uri baseUri = new(uri);

            // Act
            Uri actualResult = baseUri.Append(paths);

            // Assert
            actualResult.AbsoluteUri.Should().Be(expectedResult);
        }

        /// <summary>
        /// Appends the empty paths should return base URI.
        /// </summary>
        [TestMethod]
        public void Append_EmptyPaths_ShouldReturnBaseUri()
        {
            // Arrange
            Uri baseUri = new Uri("https://example.com");
            string[] paths = [];

            // Act
            Uri result = baseUri.Append(paths);

            // Assert
            result.Should().Be(baseUri);
        }

        /// <summary>
        /// Appends the null base URI should throw argument null exception.
        /// </summary>
        [TestMethod]
        public void Append_NullBaseUri_ShouldThrowArgumentNullException()
        {
            // Arrange
            Uri baseUri = null!;
            string[] paths = ["path1", "path2"];

            // Act & Assert
            Action action = () => baseUri.Append(paths);
            action.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("uri");
        }

        /// <summary>
        /// Appends the null paths should throw argument null exception.
        /// </summary>
        [TestMethod]
        public void Append_NullPaths_ShouldThrowArgumentNullException()
        {
            // Arrange
            Uri baseUri = new("https://example.com");
            string[] paths = null!;

            // Act & Assert
            Action action = () => baseUri.Append(paths);
            action.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("paths");
        }
    }
}