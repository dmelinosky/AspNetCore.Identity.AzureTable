// <copyright file="UserClaimTableAccess.cs" company="Gobie74">
// Copyright (c) Gobie74. All rights reserved.
// </copyright>

namespace Gobie74.AspNetCore.Identity.AzureTable
{
    using Gobie74.AzureStorage;
    using Microsoft.Azure.Cosmos.Table;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Table access for the UserClaim entity.
    /// </summary>
    public class UserClaimTableAccess : TableAccess<UserClaim>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserClaimTableAccess"/> class.
        /// </summary>
        /// <param name="table">table to use for storage.</param>
        /// <param name="logger">an optional logger.</param>
        public UserClaimTableAccess(CloudTable table, ILogger<TableAccess<UserClaim>> logger = null)
            : base(table, logger)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserClaimTableAccess"/> class.
        /// </summary>
        /// <param name="connectionString">a connection string.</param>
        /// <param name="tableName">a table name.</param>
        /// <param name="logger">an optional logger.</param>
        public UserClaimTableAccess(string connectionString, string tableName, ILogger<TableAccess<UserClaim>> logger = null)
            : base(connectionString, tableName, logger)
        {
        }
    }
}
