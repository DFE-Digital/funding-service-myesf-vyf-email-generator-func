// <copyright file="ConfigurationSettingsRegistration.cs" company="Department for Education - Skill Funding Services">
// Copyright (c) Department for Education - Skill Funding Services. All rights reserved.
// </copyright>

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Pds.Core.DfESignIn.Models;
using Pds.VYF.EmailGenerator.Services.Extensions;
using Pds.VYF.EmailGenerator.Services.Models.ConfigurationSettings;

namespace Pds.VYF.EmailGenerator.Services.ServiceRegistrations
{
    /// <summary>
    /// The class for ConfigurationSettingsRegistration.
    /// </summary>
    public static class ConfigurationSettingsRegistration
    {
        private const string DSIPublicApiOptionName = "DfESignin:PublicApi";

        /// <summary>
        /// Registers the configuration settings.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>IServiceCollection.</returns>
        public static IServiceCollection RegisterConfigurationSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<CosmosSettings>()
                    .BindConfiguration(CosmosSettings.OptionName)
                    .ValidateDataAnnotations().ValidateOnStart()
                    .ResolveConfig(services, CosmosSettings.OptionName);

            services.AddOptions<AzureTableSettings>()
                    .BindConfiguration(AzureTableSettings.OptionName)
                    .ValidateDataAnnotations().ValidateOnStart()
                    .ResolveConfig(services, AzureTableSettings.OptionName);

            services.AddOptions<AppSettings>()
                    .BindConfiguration(AppSettings.OptionName)
                    .ValidateDataAnnotations().ValidateOnStart()
                    .ResolveConfig(services, AppSettings.OptionName);

            services.AddOptions<VYFUISettings>()
                    .BindConfiguration(VYFUISettings.OptionName)
                    .ValidateDataAnnotations().ValidateOnStart()
                    .ResolveConfig(services, VYFUISettings.OptionName);

            services.AddOptions<PublicApiSettings>()
                    .BindConfiguration(DSIPublicApiOptionName)
                    .ResolveConfig(services, DSIPublicApiOptionName);

            return services;
        }
    }
}
