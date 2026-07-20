// <copyright file="VYFUIServices.cs" company="Department for Education - Skill Funding Services">
// Copyright (c) Department for Education - Skill Funding Services. All rights reserved.
// </copyright>

using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Pds.VYF.EmailGenerator.Services.Abstract.ApiConnectors;
using Pds.VYF.EmailGenerator.Services.Extensions;
using Pds.VYF.EmailGenerator.Services.Models.ConfigurationSettings;
using Pds.VYF.EmailGenerator.Services.Models.Requests;
using Pds.VYF.EmailGenerator.Services.Models.Responses;
using System.Net.Http.Headers;

namespace Pds.VYF.EmailGenerator.Services.Services.ApiConnectors
{
    /// <summary>
    /// The class for VYFUIServices.
    /// </summary>
    /// <seealso cref="Pds.VYF.EmailGenerator.Services.Abstract.ApiConnectors.IVYFUIServices" />
    public class VYFUIServices : IVYFUIServices
    {
        private const string SecureTokenHeaderName = "x-secret-key";
        private const string JsonMediaType = "application/json";

        private readonly ILogger<VYFUIServices> logger;
        private readonly VYFUISettings vYFUISettings;
        private readonly HttpClient httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="VYFUIServices" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="vYFUISettings">The v yfui settings.</param>
        /// <param name="httpClient">The HTTP client.</param>
        /// <exception cref="System.ArgumentNullException">vYFUISettingsOption.</exception>
        public VYFUIServices(
                        ILogger<VYFUIServices> logger,
                        VYFUISettings vYFUISettings,
                        HttpClient httpClient)
        {
            this.logger = logger;
            this.vYFUISettings = vYFUISettings;
            this.httpClient = httpClient;

            this.SetupHttpClient();
        }

        /// <summary>
        /// Gets the Email Enabled funding stream and periods asynchronous.
        /// </summary>
        /// <returns>
        /// List of <see cref="EmailEnabledFundingStreamAndPeriodsResponse"></see>.
        /// </returns>
        public async Task<List<EmailEnabledFundingStreamAndPeriodsResponse>?> GetEmailEnabledFundingStreamAndPeriodsAsync()
        {
            var url = $"/{this.vYFUISettings.EmailEnabledFundingStreamAndPeriodsEndpointUri}";
            return await this.GetApiResult<List<EmailEnabledFundingStreamAndPeriodsResponse>?>(url);
        }

        /// <summary>
        /// Gets the latest funding stream published date.
        /// </summary>
        /// <param name="processRequest">The process request.</param>
        /// <returns>
        /// A <see cref="Task" /> representing the asynchronous operation.
        /// </returns>
        public async Task<DateTime?> GetLatestFundingStreamPublishedDate(ProcessRequest processRequest)
        {
            var url = this.vYFUISettings.LatestFundingStreamPublishedDateEndpointUri.AppendForUri(processRequest.FundingStreamCode, processRequest.FundingPeriodId);
            return await this.GetApiResult<DateTime?>(url.ToString());
        }

        private void SetupHttpClient()
        {
            this.httpClient.BaseAddress = new Uri(this.vYFUISettings.BaseUri);
            this.httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue(JsonMediaType));
            this.httpClient.DefaultRequestHeaders.Add(SecureTokenHeaderName, this.vYFUISettings.ApiKey);
        }

        /// <summary>
        /// Gets the API result.
        /// </summary>
        /// <typeparam name="T">Any type.</typeparam>
        /// <param name="url">The URL.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        private async Task<T?> GetApiResult<T>(string url)
        {
            var httpResponse = await this.httpClient.GetAsync(url);

            if (!httpResponse.IsSuccessStatusCode)
            {
                this.logger.LogError($"Error while getting response from {url} with status code: {httpResponse.StatusCode}, Reason Phrase: {httpResponse.ReasonPhrase}");
                return default;
            }

            var responseContentString = await httpResponse.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<T>(responseContentString);

            return result;
        }
    }
}
