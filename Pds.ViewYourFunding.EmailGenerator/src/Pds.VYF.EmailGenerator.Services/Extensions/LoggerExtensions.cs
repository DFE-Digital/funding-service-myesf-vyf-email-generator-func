// <copyright file="LoggerExtensions.cs" company="Department for Education - Skill Funding Services">
// Copyright (c) Department for Education - Skill Funding Services. All rights reserved.
// </copyright>

using Microsoft.Extensions.Logging;
using Pds.VYF.EmailGenerator.Services.Extensions;
using Pds.VYF.EmailGenerator.Services.Models.Requests;
using System.Runtime.CompilerServices;
using System.Text;

namespace Pds.VYF.EmailGenerator.Services.Extensions
{
    /// <summary>
    /// Logger Extensions.
    /// </summary>
    public static class LoggerExtensions
    {
        /// <summary>
        /// Logs the information.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="processRequest">The process request.</param>
        /// <param name="moduleName">Name of the module.</param>
        /// <param name="message">The message.</param>
        /// <param name="timeTaken">The time taken.</param>
        /// <param name="keyValues">The key values.</param>
        public static void LogInformation(
                                this ILogger logger,
                                ProcessRequest processRequest,
                                string moduleName,
                                string message,
                                TimeSpan? timeTaken = null,
                                params (string Key, object Value)[] keyValues)
        {
            var logData = processRequest.GetLogData();
            var sb = processRequest.GetStringBuilder();

            sb.AddIfExists(logData, moduleName);
            sb.AddIfExists(logData, timeTaken);

            foreach (var keyValue in keyValues)
            {
                sb.AddIfExists(logData, keyValue.Value, keyValue.Key);
            }

            sb.AddIfExists(logData, message);

            logger.LogInformation(sb.ToString(), logData.ToArray());
        }

        /// <summary>
        /// Log Error.
        /// </summary>
        /// <param name="logger">logger.</param>
        /// <param name="processRequest">processRequest.</param>
        /// <param name="moduleName">moduleName.</param>
        /// <param name="exception">exception.</param>
        /// <param name="errorMessage">errorMessage.</param>
        /// <param name="keyValues">The key values.</param>
        public static void LogError(
                                this ILogger logger,
                                ProcessRequest processRequest,
                                string moduleName,
                                Exception? exception = null,
                                string? errorMessage = null,
                                params (string Key, object Value)[] keyValues)
        {
            var logData = processRequest.GetLogData();
            var sb = processRequest.GetStringBuilder();
            var message = exception?.Message ?? errorMessage ?? "Unknown Error. Please check further logs for more info";

            sb.AddIfExists(logData, moduleName);
            sb.AddIfExists(logData, message);

            foreach (var (key, value) in keyValues)
            {
                sb.AddIfExists(logData, value, key);
            }

            logger.LogError(exception, sb.ToString(), [.. logData]);
        }

        private static void AddIfExists<T>(this StringBuilder sb, List<object> logData, T? value, [CallerArgumentExpression(nameof(value))] string? key = null)
        {
            key = key?.FirstCharToUpper() ?? throw new ArgumentNullException(nameof(key));

            if (value is string stringValue && !string.IsNullOrWhiteSpace(stringValue))
            {
                sb.Append($", {key?.AddSpacesToSentence()}: {{{key}}}");
                logData.Add(stringValue);
            }
            else if (value != null)
            {
                sb.Append($", {key?.AddSpacesToSentence()}: {{{key}}}");
                logData.Add(value);
            }
        }

        private static StringBuilder GetStringBuilder(this ProcessRequest processRequest)
                                    => new StringBuilder("Funding Stream: {FundingStreamCode}, Funding Period: {FundingPeriodId}, Email Type: {EmailTypes}");

        private static List<object> GetLogData(this ProcessRequest processRequest)
                                    => [processRequest.FundingStreamCode, processRequest.FundingPeriodId, processRequest.EmailTypes];
    }
}
