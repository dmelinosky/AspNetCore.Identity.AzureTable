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

        /// <summary>
        /// Find a user by searching for a specified value in a specified property.
        /// </summary>
        /// <param name="propertyName">the property name. </param>
        /// <param name="propertyValue"> the property value. </param>
        /// <returns> A User if found, null if not. </returns>
        public virtual async Task<Role> FindSingleByProperty(string propertyName, string propertyValue)
        {
            await this.CreateIfNotExists();

            TableQuery<Role> query = new TableQuery<Role>().Where(
                TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, Role.RowKeyIdentifier),
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition(propertyName, QueryComparisons.Equal, propertyValue)));

            var stuffs = await this.CloudTable.ExecuteQuerySegmentedAsync(query, new TableContinuationToken());

            Role foundUser = null;

            foreach (Role thing in stuffs.Results)
            {
                foundUser = thing;
                break;
            }

            return foundUser;
        }
    }
}
