// <copyright file="OptionsBuilderExtensions.cs" company="Department for Education - Skill Funding Services">
// Copyright (c) Department for Education - Skill Funding Services. All rights reserved.
// </copyright>

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Pds.VYF.EmailGenerator.Services.Extensions
{
    /// <summary>
    /// The Extension class for OptionsBuilder.
    /// </summary>
    public static class OptionsBuilderExtensions
    {
        /// <summary>
        /// Resolves the configuration.
        /// </summary>
        /// <typeparam name="T">Any Type.</typeparam>
        /// <param name="optionsBuilder">The options builder.</param>
        /// <param name="services">The services.</param>
        /// <param name="optionName">Name of the option.</param>
        /// <returns>The same OptionsBuilder for Chaining.</returns>
        public static OptionsBuilder<T> ResolveConfig<T>(this OptionsBuilder<T> optionsBuilder, IServiceCollection services, string optionName)
            where T : class
        {
            services.AddSingleton(resolver =>
            {
                var result = resolver.GetService<IOptions<T>>()?.Value ?? throw new ArgumentNullException($"{optionName} is missing!");
                return result;
            });

            return optionsBuilder;
        }
    }
}
