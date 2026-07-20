// <copyright file="StringExtensions.cs" company="Department for Education - Skill Funding Services">
// Copyright (c) Department for Education - Skill Funding Services. All rights reserved.
// </copyright>

using System.Text;

namespace Pds.VYF.EmailGenerator.Services.Extensions
{
    /// <summary>
    /// The helper class for StringExtensions.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Adds the quote in each value.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="delimiter">The delimiter.</param>
        /// <param name="isSingleQuote">if set to <c>true</c> [is single quote].</param>
        /// <returns>string.</returns>
        public static string AddQuoteInEachValue(this string? input, string delimiter, bool isSingleQuote)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return string.Empty;
            }

            string quote = isSingleQuote ? "'" : "\"";
            var quoatedEntries = input.Split(delimiter, StringSplitOptions.RemoveEmptyEntries).Select(a => quote + a + quote);
            return string.Join(delimiter, quoatedEntries);
        }

        /// <summary>
        /// Firsts the character to upper.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>string with first letter as Capital Letter. Remain letters will be not be changed.</returns>
        /// <exception cref="ArgumentNullException">input.</exception>
        /// <exception cref="ArgumentException">if input is empty or only having whitespace.</exception>
        public static string FirstCharToUpper(this string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return string.Empty;
            }

            return string.Concat(input[0].ToString().ToUpper(), input.AsSpan(1));
        }

        /// <summary>
        /// Adds the spaces to sentence.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="preserveAcronyms">if set to <c>true</c> [preserve acronyms].</param>
        /// <returns>Added Spaces between each words (Pascal Case words).</returns>
        public static string AddSpacesToSentence(this string? text, bool preserveAcronyms = true)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            StringBuilder newText = new StringBuilder(text.Length * 2);
            newText.Append(text[0]);
            for (int i = 1; i < text.Length; i++)
            {
                char currentChar = text[i];
                char previousChar = text[i - 1];
                char? nextChar = i < text.Length - 1 ? text[i + 1] : null;

                if (char.IsUpper(currentChar))
                {
                    if (char.IsUpper(previousChar))
                    {
                        if ((preserveAcronyms && nextChar is not null && !char.IsUpper(nextChar.Value)) || !preserveAcronyms)
                        {
                            newText.Append(' ');
                        }
                    }
                    else if (text[i - 1] != ' ')
                    {
                        newText.Append(' ');
                    }
                }

                newText.Append(text[i]);
            }

            return newText.ToString();
        }

        /// <summary>
        /// Appends for URI.
        /// </summary>
        /// <param name="seed">The first part.</param>
        /// <param name="paths">The paths.</param>
        /// <returns>
        /// Append Uri as string.
        /// </returns>
        public static string AppendForUri(this string seed, params string[] paths)
        {
            return paths.Aggregate(seed, (first, second) => string.Format("{0}/{1}", first.Trim('/'), second.Trim('/')));
        }
    }
}
