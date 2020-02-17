// <copyright file="UserTableAccess.cs" company="Gobie74">
// Copyright (c) Gobie74. All rights reserved.
// </copyright>

namespace Gobie74.AspNetCore.Identity.AzureTable
{
    using Gobie74.AzureStorage;

    /// <summary>
    /// Table access for the user entity.
    /// </summary>
    internal class UserTableAccess : TableAccess<User>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserTableAccess"/> class.
        /// </summary>
        /// <param name="connectionString"> azure table connection string. </param>
        /// <param name="tableName"> table name to use for storage. </param>
        public UserTableAccess(string connectionString, string tableName)
            : base(connectionString, tableName)
        {
        }
    }
}
