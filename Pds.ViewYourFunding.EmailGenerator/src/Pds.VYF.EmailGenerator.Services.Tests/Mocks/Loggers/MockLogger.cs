using Microsoft.Extensions.Logging;
using Moq;

namespace Pds.VYF.EmailGenerator.Services.Tests.Mocks.Loggers
{
    /// <summary>
    /// The mock class for ILogger.
    /// </summary>
    /// <seealso cref="Moq.Mock&lt;Castle.Core.Logging.ILogger&gt;" />
    /// <remarks>
    /// Initializes a new instance of the <see cref="MockLogger" /> class.
    /// </remarks>
    /// <param name="isSetupDefault">if set to <c>true</c> [is setup default].</param>
    public class MockLogger(bool isSetupDefault = true) : MockBase<ILogger>(isSetupDefault)
    {
        /// <summary>
        /// Mocks the log.
        /// </summary>
        public override void SetupDefault()
        {
            this.Setup(logger => logger.Log(
                                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
                                    It.IsAny<EventId>(),
                                    It.IsAny<It.IsAnyType>(),
                                    It.IsAny<Exception>(),
                                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()));

            this.Setup(logger => logger.Log(
                                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                                    It.IsAny<EventId>(),
                                    It.IsAny<It.IsAnyType>(),
                                    It.IsAny<Exception>(),
                                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
        }

        /// <summary>
        /// Verifies the log.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="times">The times.</param>
        /// <returns>The same object for chaining.</returns>
        public MockLogger VerifyLog(LogLevel logLevel, Times times)
        {
            this.Verify(
                        logger => logger.Log(
                                It.Is<LogLevel>(l => l == logLevel),
                                It.IsAny<EventId>(),
                                It.IsAny<It.IsAnyType>(),
                                It.IsAny<Exception>(),
                                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                        times);

            return this;
        }

        /// <summary>
        /// Verifies the log.
        /// </summary>
        /// <typeparam name="T">Any Type.</typeparam>
        /// <param name="logLevel">The log level.</param>
        /// <param name="message">The message.</param>
        /// <param name="times">The times.</param>
        /// <returns>The same object for chaining.</returns>
        public MockLogger VerifyLog(LogLevel logLevel, string message, Times times)
        {
            this.Verify(
                        logger => logger.Log(
                                It.Is<LogLevel>(l => l == logLevel),
                                It.IsAny<EventId>(),
                                It.Is<It.IsAnyType>((v, t) => Convert.ToString(v).Contains(message)),
                                It.IsAny<Exception>(),
                                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                        times);

            return this;
        }
    }
}
