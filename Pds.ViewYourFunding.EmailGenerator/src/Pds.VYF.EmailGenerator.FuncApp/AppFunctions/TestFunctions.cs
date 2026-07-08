// <copyright file="TestFunctions.cs" company="Department for Education - Skill Funding Services">
// Copyright (c) Department for Education - Skill Funding Services. All rights reserved.
// </copyright>

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Pds.Core.DfESignIn.Models;
using Pds.VYF.EmailGenerator.Services.Abstract.Controllers;
using Pds.VYF.EmailGenerator.Services.Abstract.Processors;

namespace Pds.VYF.EmailGenerator.FuncApp.AppFunctions
{
    /// <summary>
    /// The class for TestFunctions.
    /// </summary>
    public class TestFunctions(ILogger<TestFunctions> logger, IEmailPublisher emailPublisher, PublicApiSettings publicAPISettings, IJobControllerServices jobController)
    {
        /// <summary>
        /// Runs the specified req.
        /// </summary>
        /// <param name="req">The req.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [Function("TestGetEmailIdsFromDSIFunction")]
        public async Task<IActionResult> RunGetEmailIdsFromDSI([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            logger.LogInformation("C# HTTP trigger function processed a request.");

            var emailAddresses = await emailPublisher.GetEmailAddresses("10058293");

            return new OkObjectResult($"The Email Addresses are {string.Join("|", emailAddresses)} with secret {publicAPISettings.ClientSecret}");
        }

        /// <summary>
        /// Runs the parents alone.
        /// </summary>
        /// <param name="req">The req.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [Function("TestParentsEmailFunction")]
        public async Task<IActionResult> RunParentsAlone([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req, CancellationToken cancellationToken)
        {
            await jobController.RunForAFundingStreamAndPeriod(Services.Enumerations.EmailTypesEnum.ForParents, "GAG", "General annual grant", "AC-2425", new DateTime(2024, 1, 1), cancellationToken);

            return new OkObjectResult($"The Parents Email processing completed!");
        }

        /// <summary>
        /// Runs the children alone.
        /// </summary>
        /// <param name="req">The req.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [Function("TestChildrenEmailFunction")]
        public async Task<IActionResult> RunChildrenAlone([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req, CancellationToken cancellationToken)
        {
            await jobController.RunForAFundingStreamAndPeriod(Services.Enumerations.EmailTypesEnum.ForChildren, "GAG", "General annual grant", "AC-2425", new DateTime(2024, 1, 1), cancellationToken);

            return new OkObjectResult($"The Children Email processing completed!");
        }
    }
}
