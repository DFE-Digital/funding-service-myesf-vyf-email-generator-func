// <copyright file="EmailTemplatesModelExtensionTests.cs" company="Department for Education - Skill Funding Services">
// Copyright (c) Department for Education - Skill Funding Services. All rights reserved.
// </copyright>

using FluentAssertions;
using Pds.VYF.EmailGenerator.Services.Models.ConfigurationSettings;

namespace Pds.VYF.EmailGenerator.Services.Helpers.Tests
{
    /// <summary>
    /// The Test Class for EmailTemplatesModelExtension.
    /// </summary>
    [TestClass]
    public class EmailTemplatesModelExtensionTests
    {
        /// <summary>
        /// Gets all email message types test.
        /// </summary>
        [TestMethod]
        public void GetAllEmailMessageTypesTest()
        {
            // Arrange
            var emailTemplatesModel = new EmailTemplatesModel();
            var expectedResult = new List<string>()
            {
                "ChildNewFunding",
                "ChildUpdatedFunding",
                "ParentNewFundings",
                "ParentUpdatedFundings",
                "ParentNewAndUpdatedFundings",
            };

            // Act
            var actualResult = emailTemplatesModel.GetAllEmailMessageTypes();

            // Assert
            actualResult.Should()
                .NotBeEmpty()
                .And.HaveCount(5)
                .And.Equal(expectedResult);
        }

        /// <summary>
        /// Gets the email message type test.
        /// </summary>
        [TestMethod]
        public void GetEmailMessageTypeTest()
        {
            // Arrange
            var expectedResult = "ChildNewFunding";

            // Act
            var actualResult = EmailTemplatesModelExtension.GetEmailMessageType(a => a.ChildNewFunding);

            // Assert
            actualResult.Should().Be(expectedResult);
        }

        /// <summary>
        /// Gets the template identifier test.
        /// </summary>
        [TestMethod]
        public void GetTemplateIdTest()
        {
            // Arrange
            var expectedResult = "TestValue";
            var emailTemplatesModel = new EmailTemplatesModel()
            {
                ChildNewFunding = expectedResult,
            };

            // Act
            var actualResult = emailTemplatesModel.GetTemplateId("ChildNewFunding");

            // Assert
            actualResult.Should().Be(expectedResult);
        }
    }
}