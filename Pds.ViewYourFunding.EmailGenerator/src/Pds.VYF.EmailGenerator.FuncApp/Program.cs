// <copyright file="Program.cs" company="Department for Education - Skill Funding Services">
// Copyright (c) Department for Education - Skill Funding Services. All rights reserved.
// </copyright>

using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pds.VYF.EmailGenerator.Services.Extensions;
using Pds.VYF.EmailGenerator.Services.ServiceRegistrations;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;

        services.AddLogging(logging => logging.ClearProviders().AddSimpleConsole(c => c.SingleLine = true));

        services.AddApplicationInsightsTelemetryWorkerService(configuration);
        services.ConfigureFunctionsApplicationInsights();

        services.RegisterConfigurationSettings(configuration);
        services.RegisterAppServices(configuration);
        services.RegisterInfraServices(configuration);
    })
    .Build();

host.SeedAndRun<Program>();