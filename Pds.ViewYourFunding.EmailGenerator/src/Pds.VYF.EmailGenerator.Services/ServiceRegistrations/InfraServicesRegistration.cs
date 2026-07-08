// <copyright file="InfraServicesRegistration.cs" company="Department for Education - Skill Funding Services">
// Copyright (c) Department for Education - Skill Funding Services. All rights reserved.
// </copyright>

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pds.Core.Caching;
using Pds.Core.DfESignIn.Interfaces;
using Pds.Core.DfESignIn.Services;
using Pds.Core.Notification;
using Pds.Core.Notification.Registration;
using Pds.VYF.EmailGenerator.Services.Abstract.ApiConnectors;
using Pds.VYF.EmailGenerator.Services.Abstract.InfraServices;
using Pds.VYF.EmailGenerator.Services.Services.ApiConnectors;
using Pds.VYF.EmailGenerator.Services.Services.InfraServices;
using Polly;

namespace Pds.VYF.EmailGenerator.Services.ServiceRegistrations
{
    /// <summary>
    /// Cosmos Client Registration.
    /// </summary>
    public static class InfraServicesRegistration
    {
        /// <summary>
        /// Registers the cosmos client services.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>
        ///   <see cref="IServiceCollection" />.
        /// </returns>
        public static IServiceCollection RegisterInfraServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IAzureTableServices, AzureTableServices>();
            services.AddSingleton<ICosmosQueryServices, CosmosQueryServices>();
            services.AddSingleton<ICosmosContainerServices, CosmosContainerServices>();

            services.AddHttpClient<IVYFUIServices, VYFUIServices>()
                    .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));

            // Configure PDS Packages Shared Components.
            services.AddNotificationClient(a => configuration.Bind(nameof(ServiceBusClientConfiguration), a));

            services.AddHttpClient<IDfESignInPublicApi, DfESignInPublicApi>()
                    .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));

            services.AddPdsMemoryCache();

            return services;
        }
    }
}
