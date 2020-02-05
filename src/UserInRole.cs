// <copyright file="UserInRole.cs" company="Gobie74">
// Copyright (c) Gobie74. All rights reserved.
// </copyright>

namespace Gobie74.AspNetCore.Identity.AzureTable
{
    using System;
    using Microsoft.Azure.Cosmos.Table;

    /// <summary>
    /// Storage for a user / role membership.
    /// </summary>
    public class UserInRole : TableEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserInRole"/> class.
        /// </summary>
        public UserInRole()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserInRole"/> class.
        /// </summary>
        /// <param name="role">the role id.</param>
        /// <param name="user">the user id.</param>
        public UserInRole(Guid role, Guid user)
        {
            this.PartitionKey = role.ToString();
            this.RowKey = user.ToString();
        }

        /// <summary>
        /// Gets or sets the date and time the user was added to the role.
        /// </summary>
        public DateTime AddedUtc { get; set; }

        /// <summary>
        /// Gets or sets the user that added the user to the role.
        /// </summary>
        public string AddedBy { get; set; }

        /// <summary>
        /// Gets the user id.
        /// </summary>
        [IgnoreProperty]
        public Guid User
        {
            get
            {
                return Guid.Parse(this.RowKey);
            }
        }

        /// <summary>
        /// Gets the role id.
        /// </summary>
        [IgnoreProperty]
        public Guid Role
        {
            get
            {
                return Guid.Parse(this.PartitionKey);
            }
        }
    }
}
