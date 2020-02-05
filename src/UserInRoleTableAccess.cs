// <copyright file="UserInRoleTableAccess.cs" company="Gobie74">
// Copyright (c) Gobie74. All rights reserved.
// </copyright>

namespace Gobie74.AspNetCore.Identity.AzureTable
{
    using Gobie74.AzureStorage;

    /// <summary>
    /// Data storage accessor for UserInRole entities.
    /// </summary>
    public class UserInRoleTableAccess : TableAccess<UserInRole>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserInRoleTableAccess"/> class.
        /// </summary>
        /// <param name="connectionString">the connection string.</param>
        /// <param name="tableName">the table name.</param>
        public UserInRoleTableAccess(string connectionString, string tableName = "role")
            : base(connectionString, tableName)
        {
        }
    }
}
