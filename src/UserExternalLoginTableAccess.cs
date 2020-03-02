// <copyright file="UserExternalLoginTableAccess.cs" company="Gobie74">
// Copyright (c) Gobie74. All rights reserved.
// </copyright>

namespace Gobie74.AspNetCore.Identity.AzureTable
{
    using Gobie74.AzureStorage;

    /// <summary>
    /// Table access for external login information.
    /// </summary>
    public class UserExternalLoginTableAccess : TableAccess<UserExternalLogin>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserExternalLoginTableAccess"/> class.
        /// </summary>
        /// <param name="table">the cloud table to use.</param>
        /// <param name="logger">an optional logger.</param>
        public UserExternalLoginTableAccess(Microsoft.Azure.Cosmos.Table.CloudTable table, Microsoft.Extensions.Logging.ILogger<TableAccess<UserExternalLogin>> logger = null)
            : base(table, logger)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserExternalLoginTableAccess"/> class.
        /// </summary>
        /// <param name="connectionString">a connection string.</param>
        /// <param name="tableName">a table name.</param>
        /// <param name="logger">an optional logger.</param>
        public UserExternalLoginTableAccess(string connectionString, string tableName, Microsoft.Extensions.Logging.ILogger<TableAccess<UserExternalLogin>> logger = null)
            : base(connectionString, tableName, logger)
        {
        }
    }
}
