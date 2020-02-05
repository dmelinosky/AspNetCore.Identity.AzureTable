// <copyright file="UserTableAccess.cs" company="Gobie74">
// Copyright (c) Gobie74. All rights reserved.
// </copyright>

namespace Gobie74.AspNetCore.Identity.AzureTable
{
    using System.Threading.Tasks;

    using Gobie74.AzureStorage;
    using Microsoft.Azure.Cosmos.Table;

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

        /// <summary>
        /// Find a user by searching for a specified value in a specified property.
        /// </summary>
        /// <param name="propertyName">the property name. </param>
        /// <param name="propertyValue"> the property value. </param>
        /// <returns> A User if found, null if not. </returns>
        public virtual async Task<User> FindSingleByProperty(string propertyName, string propertyValue)
        {
            await this.CreateIfNotExists();

            TableQuery<User> query = new TableQuery<User>().Where(
                TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, User.UserDataRowKey),
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition(propertyName, QueryComparisons.Equal, propertyValue)));

            var stuffs = await this.CloudTable.ExecuteQuerySegmentedAsync(query, new TableContinuationToken());

            User foundUser = null;

            foreach (User thing in stuffs.Results)
            {
                foundUser = thing;
                break;
            }

            return foundUser;
        }
    }
}
