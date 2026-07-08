using Microsoft.Extensions.Logging;
using Moq;

namespace Pds.VYF.EmailGenerator.Services.Tests.Mocks.Loggers
{
    /// <summary>
    /// The mock class for ILogger.
    /// </summary>
    /// <typeparam name="T">Any type.</typeparam>
    /// <seealso cref="Mock&lt;Castle.Core.Logging.ILogger&lt;T&gt;&gt;" />
    public class MockLogger<T>(bool isSetupDefault = true) : MockBase<ILogger<T>>(isSetupDefault)
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
        /// <typeparam name="T">Any Type.</typeparam>
        /// <param name="logLevel">The log level.</param>
        /// <param name="message">The message.</param>
        /// <param name="times">The times.</param>
        /// <returns>The same object for chaining.</returns>
        public MockLogger<T> VerifyLog(LogLevel logLevel, string message, Times times)
        {
            this.Verify(
                        logger => logger.Log(
                                It.Is<LogLevel>(l => l == logLevel),
                                It.IsAny<EventId>(),
                                It.Is<It.IsAnyType>((v, t) => v != null && v.ToString().Contains(message)),
                                It.IsAny<Exception>(),
                                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                        times);

            return this;
        }

    }
}
