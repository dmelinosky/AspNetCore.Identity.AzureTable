// <copyright file="UserExternalLogin.cs" company="Gobie74">
// Copyright (c) Gobie74. All rights reserved.
// </copyright>

namespace Gobie74.AspNetCore.Identity.AzureTable
{
    using System;
    using Microsoft.Azure.Cosmos.Table;

    /// <summary>
    /// Stores external login information for a user.
    /// </summary>
    public class UserExternalLogin : TableEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserExternalLogin"/> class.
        /// </summary>
        public UserExternalLogin()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserExternalLogin"/> class.
        /// </summary>
        /// <param name="user">the user id.</param>
        /// <param name="provider">the provider name.</param>
        public UserExternalLogin(Guid user, string provider)
        {
            this.PartitionKey = user.ToString();
            this.RowKey = provider;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserExternalLogin"/> class.
        /// </summary>
        /// <param name="user">the user id.</param>
        /// <param name="provider">the provider name.</param>
        public UserExternalLogin(string user, string provider)
        {
            this.PartitionKey = user;
            this.RowKey = provider;
        }

        /// <summary>
        /// Gets or sets a provider key.
        /// </summary>
        public string ProviderKey { get; set; }

        /// <summary>
        /// Gets or sets a provider display name.
        /// </summary>
        public string DisplayName { get; set; }
    }
}
