// <copyright file="AppServicesRegistration.cs" company="Department for Education - Skill Funding Services">
// Copyright (c) Department for Education - Skill Funding Services. All rights reserved.
// </copyright>

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pds.VYF.EmailGenerator.Services.Abstract.ApiConnectors;
using Pds.VYF.EmailGenerator.Services.Abstract.Controllers;
using Pds.VYF.EmailGenerator.Services.Abstract.Processors;
using Pds.VYF.EmailGenerator.Services.Services.ApiConnectors;
using Pds.VYF.EmailGenerator.Services.Services.Controllers;
using Pds.VYF.EmailGenerator.Services.Services.Processors;

namespace Pds.VYF.EmailGenerator.Services.ServiceRegistrations
{
    /// <summary>
    /// The class for AppServicesRegistration.
    /// </summary>
    public static class AppServicesRegistration
    {
        /// <summary>
        /// Registers the application services.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns><see cref="IServiceCollection">Services</see>/>.</returns>
        public static IServiceCollection RegisterAppServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IJobControllerServices, JobControllerServices>();
            services.AddSingleton<IAuditAndControlServices, AuditAndControlServices>();

            services.AddSingleton<IChildProcessor, ChildProcessor>();
            services.AddSingleton<IParentProcessor, ParentProcessor>();
            services.AddSingleton<IEmailPublisher, EmailPublisher>();

            services.AddSingleton<IDataSeedServices, DataSeedServices>();

            return services;
        }
    }
}
