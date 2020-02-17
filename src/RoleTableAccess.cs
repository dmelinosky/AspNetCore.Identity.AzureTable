// <copyright file="RoleTableAccess.cs" company="Gobie74">
// Copyright (c) Gobie74. All rights reserved.
// </copyright>

namespace Gobie74.AspNetCore.Identity.AzureTable
{
    using System.Threading.Tasks;

    using Gobie74.AzureStorage;
    using Microsoft.Azure.Cosmos.Table;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Role access from table storage.
    /// </summary>
    public class RoleTableAccess : TableAccess<Role>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoleTableAccess"/> class.
        /// </summary>
        /// <param name="connectionString">azure table connection string.</param>
        /// <param name="tableName">table name to use for storage.</param>
        /// <param name="logger">an optional logger.</param>
        public RoleTableAccess(string connectionString, string tableName, ILogger<RoleTableAccess> logger = null)
            : base(connectionString, tableName)
        {
            logger?.LogTrace("RoleTableAccess construction.");
        }
    }
}
