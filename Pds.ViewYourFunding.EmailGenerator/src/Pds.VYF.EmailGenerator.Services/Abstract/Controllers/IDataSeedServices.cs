// <copyright file="IDataSeedServices.cs" company="Department for Education - Skill Funding Services">
// Copyright (c) Department for Education - Skill Funding Services. All rights reserved.
// </copyright>

namespace Pds.VYF.EmailGenerator.Services.Abstract.Controllers
{
    /// <summary>
    /// Interface for IDataSeedServices.
    /// </summary>
    public interface IDataSeedServices
    {
        /// <summary>
        /// Seeds the asynchronous.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SeedAsync();
    }
}
