// <copyright file="HostExtensions.cs" company="Department for Education - Skill Funding Services">
// Copyright (c) Department for Education - Skill Funding Services. All rights reserved.
// </copyright>

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pds.VYF.EmailGenerator.Services.Abstract.Controllers;

namespace Pds.VYF.EmailGenerator.Services.Extensions
{
    /// <summary>
    /// The class for Host Extensions.
    /// </summary>
    public static class HostExtensions
    {
        /// <summary>
        /// Seeds the and run.
        /// </summary>
        /// <typeparam name="T">The Program Class.</typeparam>
        /// <param name="host">The host.</param>
        public static void SeedAndRun<T>(this IHost host)
        {
            var logger = host.Services.GetRequiredService<ILogger<T>>();
            Seed(host, logger);
            Run(host, logger);
        }

        private static void Run<T>(IHost host, ILogger<T> logger)
        {
            try
            {
                host.Run();
            }
            catch (AggregateException ex)
            {
                var errorMessage = "Multiple Error Occurred. Message:\n" + ex.Message;

                foreach (var exception in ex.Flatten().InnerExceptions)
                {
                    errorMessage += "\n" + ex.Message;
                }

                logger.LogError(errorMessage);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
        }

        private static void Seed<T>(IHost host, ILogger<T> logger)
        {
            try
            {
                var scope = host.Services.CreateScope();
                var dataSeedServices = scope.ServiceProvider.GetService<IDataSeedServices>();
                dataSeedServices?.SeedAsync()?.Wait();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while seed data operations. Please check further logs to find the issue.");
            }
        }
    }
}
