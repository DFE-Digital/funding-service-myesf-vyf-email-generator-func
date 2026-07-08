// <copyright file="EmailTemplatesModelExtension.cs" company="Department for Education - Skill Funding Services">
// Copyright (c) Department for Education - Skill Funding Services. All rights reserved.
// </copyright>

using System.Linq.Expressions;
using Pds.VYF.EmailGenerator.Services.Models.ConfigurationSettings;

namespace Pds.VYF.EmailGenerator.Services.Helpers
{
    /// <summary>
    /// A helper class for EmailTemplatesModelExtension.
    /// </summary>
    public static class EmailTemplatesModelExtension
    {
        /// <summary>
        /// Gets all email message types.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>All email Message types.</returns>
        public static IEnumerable<string> GetAllEmailMessageTypes(this EmailTemplatesModel obj)
        {
            return obj.GetType()
                .GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                .Select(a => a.Name);
        }

        /// <summary>
        /// Gets the type of the email message.
        /// </summary>
        /// <param name="emailMessageTypeExpression">The email message type expression.</param>
        /// <returns>Email Message Type.</returns>
        /// <exception cref="System.ArgumentException">You must pass a lambda of the form: '() => Class.Property' or '() => object.Property'.</exception>
        public static string GetEmailMessageType(Expression<Func<EmailTemplatesModel, object?>> emailMessageTypeExpression)
        {
            var me = emailMessageTypeExpression.Body as MemberExpression ?? throw new ArgumentException("You must pass a lambda of the form: '() => Class.Property' or '() => object.Property'");

            return me.Member.Name;
        }

        /// <summary>
        /// Gets the template identifier.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="emailMessageType">Type of the email message.</param>
        /// <returns>Template ID.</returns>
        public static string? GetTemplateId(this EmailTemplatesModel obj, string emailMessageType)
        {
            return Convert.ToString(obj.GetType().GetProperty(emailMessageType)?.GetValue(obj));
        }
    }
}
