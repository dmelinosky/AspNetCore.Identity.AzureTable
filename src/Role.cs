// <copyright file="Role.cs" company="Gobie74">
// Copyright (c) Gobie74. All rights reserved.
// </copyright>

namespace Gobie74.AspNetCore.Identity.AzureTable
{
    using System;
    using System.Diagnostics.Contracts;

    using Gobie74.AspNetCore.Identity.AzureTable.Contracts;
    using Microsoft.Azure.Cosmos.Table;

    /// <summary>
    /// A user role.
    /// </summary>
    [ContractClass(typeof(RoleContract))]
    public class Role : TableEntity
    {
        /// <summary>
        /// The string used in row keys for the main role information row.
        /// </summary>
        public const string RowKeyIdentifier = "metadata";

        /// <summary>
        /// Initializes a new instance of the <see cref="Role"/> class.
        /// </summary>
        public Role()
        {
            this.RowKey = RowKeyIdentifier;
            this.CreationDateUtc = DateTime.UtcNow;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Role"/> class.
        /// </summary>
        /// <param name="roleName">the name of the role.</param>
        public Role(string roleName)
        {
            this.RowKey = RowKeyIdentifier;

            this.Name = roleName;
            this.CreationDateUtc = DateTime.UtcNow;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Role"/> class.
        /// </summary>
        /// <param name="id">the role id.</param>
        public Role(Guid id)
        {
            this.PartitionKey = id.ToString();
            this.RowKey = RowKeyIdentifier;
            this.CreationDateUtc = DateTime.UtcNow;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Role"/> class.
        /// </summary>
        /// <param name="id">the role id.</param>
        /// <param name="roleName">the role name.</param>
        public Role(Guid id, string roleName)
        {
            this.PartitionKey = id.ToString();
            this.RowKey = RowKeyIdentifier;
            this.Name = roleName;
            this.CreationDateUtc = DateTime.UtcNow;
        }

        /// <summary>
        /// Gets or sets the utc creation date of the role.
        /// </summary>
        public DateTime CreationDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the role name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the role id.
        /// </summary>
        [IgnoreProperty]
        public Guid Id
        {
            get
            {
                return Guid.Parse(this.PartitionKey);
            }

            set
            {
                this.PartitionKey = value.ToString();
            }
        }

        /// <summary>
        /// Gets or sets a normalized name.
        /// </summary>
        public string NormalizedName { get; set; }
    }
}
