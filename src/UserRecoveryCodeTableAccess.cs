// <copyright file="UserRecoveryCodeTableAccess.cs" company="Gobie74">
// Copyright (c) Gobie74. All rights reserved.
// </copyright>

namespace Gobie74.AspNetCore.Identity.AzureTable
{
    using Gobie74.AzureStorage;

    /// <summary>
    /// Table access for user recover codes.
    /// </summary>
    public class UserRecoveryCodeTableAccess : TableAccess<UserRecoveryCode>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserRecoveryCodeTableAccess"/> class.
        /// </summary>
        /// <param name="connectionString"> azure table connection string. </param>
        /// <param name="tableName"> table name to use for storage. </param>
        public UserRecoveryCodeTableAccess(string connectionString, string tableName)
            : base(connectionString, tableName)
        {
        }
    }
}
