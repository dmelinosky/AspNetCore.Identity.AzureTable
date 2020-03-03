// <copyright file="UserClaim.cs" company="Gobie74">
// Copyright (c) Gobie74. All rights reserved.
// </copyright>

namespace Gobie74.AspNetCore.Identity.AzureTable
{
    using System;
    using Microsoft.Azure.Cosmos.Table;

    /// <summary>
    /// Class to hold information about a claim a user has.
    /// </summary>
    public class UserClaim : TableEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserClaim"/> class.
        /// </summary>
        public UserClaim()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserClaim"/> class.
        /// </summary>
        /// <param name="user">identifies the user.</param>
        /// <param name="claimType">the claim type.</param>
        public UserClaim(Guid user, string claimType)
        {
            this.PartitionKey = user.ToString();
            this.RowKey = claimType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserClaim"/> class.
        /// </summary>
        /// <param name="user">a Guid as a string that identities the user.</param>
        /// <param name="claimType">the claim type.</param>
        public UserClaim(string user, string claimType)
        {
            if (!Guid.TryParse(user, out Guid _))
            {
                throw new ArgumentException("The user parameter must be a Guid or a string that can be parsed into a guid.");
            }

            this.PartitionKey = user;
            this.RowKey = claimType;
        }

        /// <summary>
        /// Gets or sets the value of the claim.
        /// </summary>
        public string Value { get; set; }
    }
}
