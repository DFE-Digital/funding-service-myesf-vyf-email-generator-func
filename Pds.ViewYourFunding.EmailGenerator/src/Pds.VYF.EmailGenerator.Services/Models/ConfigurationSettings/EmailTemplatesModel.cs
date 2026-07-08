// <copyright file="EmailTemplatesModel.cs" company="Department for Education - Skill Funding Services">
// Copyright (c) Department for Education - Skill Funding Services. All rights reserved.
// </copyright>

using System.ComponentModel.DataAnnotations;

namespace Pds.VYF.EmailGenerator.Services.Models.ConfigurationSettings
{
    /// <summary>
    /// The Email Templates Model.
    /// </summary>
    public class EmailTemplatesModel
    {
        /// <summary>
        /// The option name.
        /// </summary>
        public const string OptionName = "ServiceConfiguration:EmailTemplatesModel";

        /// <summary>
        /// Gets or sets the provider new funding.
        /// </summary>
        /// <value>
        /// The provider new funding.
        /// </value>
        [Required(ErrorMessage = $"{OptionName}:{nameof(ChildNewFunding)} is missing.")]
        public string ChildNewFunding { get; set; } = default!;

        /// <summary>
        /// Gets or sets the provider updated funding.
        /// </summary>
        /// <value>
        /// The provider updated funding.
        /// </value>
        [Required(ErrorMessage = $"{OptionName}:{nameof(ChildUpdatedFunding)} is missing.")]
        public string ChildUpdatedFunding { get; set; } = default!;

        /// <summary>
        /// Gets or sets the mat new funding.
        /// </summary>
        /// <value>
        /// The mat new funding.
        /// </value>
        [Required(ErrorMessage = $"{OptionName}:{nameof(ParentNewFundings)} is missing.")]
        public string ParentNewFundings { get; set; } = default!;

        /// <summary>
        /// Gets or sets the mat updated funding.
        /// </summary>
        /// <value>
        /// The mat updated funding.
        /// </value>
        [Required(ErrorMessage = $"{OptionName}:{nameof(ParentUpdatedFundings)} is missing.")]
        public string ParentUpdatedFundings { get; set; } = default!;

        /// <summary>
        /// Gets or sets the mat new updated funding.
        /// </summary>
        /// <value>
        /// The mat new updated funding.
        /// </value>
        [Required(ErrorMessage = $"{OptionName}:{nameof(ParentNewAndUpdatedFundings)} is missing.")]
        public string ParentNewAndUpdatedFundings { get; set; } = default!;
    }
}