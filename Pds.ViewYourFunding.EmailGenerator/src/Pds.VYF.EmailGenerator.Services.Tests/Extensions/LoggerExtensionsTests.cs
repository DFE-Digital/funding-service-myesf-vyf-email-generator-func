// <copyright file="LoggerExtensionsTests.cs" company="Department for Education - Skill Funding Services">
// Copyright (c) Department for Education - Skill Funding Services. All rights reserved.
// </copyright>

using Microsoft.Extensions.Logging;
using Moq;
using Pds.VYF.EmailGenerator.Services.Extensions;
using Pds.VYF.EmailGenerator.Services.Models.Requests;
using Pds.VYF.EmailGenerator.Services.Tests.Mocks.Loggers;

namespace Pds.VYF.EmailGenerator.Services.Tests.Extensions
{
    /// <summary>
    /// The class for LogInformation Test.
    /// </summary>
    [TestClass]
    public class LoggerExtensionsTests
    {
        /// <summary>
        /// Logs the error exception provided should log error message and exception.
        /// </summary>
        [TestMethod]
        public void LogError_ExceptionProvided_ShouldLogErrorMessageAndException()
        {
            // Arrange
            var loggerMock = new MockLogger();
            var processRequest = this.CreateProcessRequest();
            var exception = new Exception("Test exception");

            // Act
            loggerMock.Object.LogError(processRequest, "ModuleA", exception: exception);

            // Assert
            loggerMock.VerifyLog(LogLevel.Error, "Funding Stream: GAG, Funding Period: FY-2324, Email Type: ForParents, Module Name: ModuleA, Message: Test exception", Times.Once());
        }

        /// <summary>
        /// Logs the error key values provided should log error message and key values.
        /// </summary>
        [TestMethod]
        public void LogError_KeyValuesProvided_ShouldLogErrorMessageAndKeyValues()
        {
            // Arrange
            var loggerMock = new MockLogger();
            var processRequest = this.CreateProcessRequest();
            (string, object)[] keyValues = [("Key1", "Value1"), ("Key2", 42)];

            // Act
            loggerMock.Object.LogError(processRequest, "ModuleA", errorMessage: "Custom error message", keyValues: keyValues);

            // Assert
            loggerMock.VerifyLog(LogLevel.Error, "Funding Stream: GAG, Funding Period: FY-2324, Email Type: ForParents, Module Name: ModuleA, Message: Custom error message, Key1: Value1, Key2: 42", Times.Once());
        }

        private ProcessRequest CreateProcessRequest()
        {
            return new ProcessRequest
            {
                FundingStreamCode = "GAG",
                FundingPeriodId = "FY-2324",
                EmailTypes = Enumerations.EmailTypesEnum.ForParents,
            };
        }
    }
}