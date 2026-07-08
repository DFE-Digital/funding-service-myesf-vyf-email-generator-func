// <copyright file="EmailFunctions.cs" company="Department for Education - Skill Funding Services">
// Copyright (c) Department for Education - Skill Funding Services. All rights reserved.
// </copyright>

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Pds.VYF.EmailGenerator.Services.Abstract.Controllers;
using System.Diagnostics;

namespace Pds.VYF.EmailGenerator.FuncApp.AppFunctions
{
    /// <summary>
    /// The class for EmailFunctions.
    /// </summary>
    public class EmailFunctions
    {
        private readonly ILogger logger;
        private readonly IJobControllerServices jobController;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailFunctions"/> class.
        /// </summary>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="jobController">The job controller.</param>
        public EmailFunctions(ILoggerFactory loggerFactory, IJobControllerServices jobController)
        {
            this.logger = loggerFactory.CreateLogger<EmailFunctions>();
            this.jobController = jobController;
        }

        /// <summary>
        /// Runs the specified my timer.
        /// </summary>
        /// <param name="myTimer">My timer.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The completed task.
        /// </returns>
        [Function("EmailTimerFunction")]
        public async Task RunTimer([TimerTrigger("%TimerInterval%")] TimerInfo myTimer, CancellationToken cancellationToken)
        {
            if (myTimer.IsPastDue)
            {
                this.logger.LogInformation("The current timer tigger is due to past due and no further action will be taken.");
                return;
            }

            await this.RunJob("VYF Email Timer function", cancellationToken);
        }

        /// <summary>
        /// Runs the specified req.
        /// </summary>
        /// <param name="req">The req.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [Function("EmailHttpFunction")]
        public async Task<IActionResult> RunHttp(
            [HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req,
            CancellationToken cancellationToken)
        {
            return await this.RunJob("Email HTTP function", cancellationToken);
        }

        private async Task<IActionResult> RunJob(string process, CancellationToken cancellationToken)
        {
            var sw = Stopwatch.StartNew();

            this.logger.LogInformation($"{process} is started...");

            try
            {
                await this.jobController.RunAsync(cancellationToken);
                this.logger.LogInformation($"{process} is successfully completed in {sw.Elapsed}.");
                return new OkObjectResult($"{process} is successfully processed.");
            }
            catch (Exception ex)
            {
                this.logger.LogError($"{process} is failed with error Message{ex.Message}");
                throw;
            }
        }
    }
}
