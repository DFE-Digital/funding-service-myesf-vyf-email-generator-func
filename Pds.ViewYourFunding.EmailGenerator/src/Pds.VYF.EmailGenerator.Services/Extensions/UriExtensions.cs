// <copyright file="UriExtensions.cs" company="Department for Education - Skill Funding Services">
// Copyright (c) Department for Education - Skill Funding Services. All rights reserved.
// </copyright>

namespace Pds.VYF.EmailGenerator.Services.Extensions
{
    /// <summary>
    /// The Extension class for URI.
    /// </summary>
    public static class UriExtensions
    {
        /// <summary>
        /// Appends the specified paths.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="paths">The paths.</param>
        /// <returns>End Uri.</returns>
        public static Uri Append(this Uri uri, params string[] paths)
        {
            ArgumentNullException.ThrowIfNull(uri);
            ArgumentNullException.ThrowIfNull(paths);

            if (paths.Any(a => string.IsNullOrWhiteSpace(a)))
            {
                throw new ArgumentException("Invalid Path");
            }

            if (!uri.IsWellFormedOriginalString())
            {
                throw new ArgumentException("Invalid URI");
            }

            return new Uri(paths.Aggregate(uri.AbsoluteUri, (current, path) => string.Format("{0}/{1}", current.TrimEnd('/'), path.TrimStart('/').TrimEnd('/'))));
        }
    }
}
