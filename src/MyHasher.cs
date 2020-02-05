// <copyright file="MyHasher.cs" company="Gobie74">
// Copyright (c) Gobie74. All rights reserved.
// </copyright>

namespace Gobie74.AspNetCore.Identity.AzureTable
{
    using Microsoft.AspNetCore.Identity;

    /// <summary>
    /// My Hasher, just for testing, doesn't actually hash.
    /// </summary>
    public class MyHasher : IPasswordHasher<User>
    {
        /// <inheritdoc/>
        public string HashPassword(User user, string password)
        {
            return password;
        }

        /// <inheritdoc/>
        public PasswordVerificationResult VerifyHashedPassword(User user, string hashedPassword, string providedPassword)
        {
            if (hashedPassword == providedPassword)
            {
                return PasswordVerificationResult.Success;
            }

            return PasswordVerificationResult.Failed;
        }
    }
}
