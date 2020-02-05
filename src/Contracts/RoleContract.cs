// <copyright file="RoleContract.cs" company="Gobie74">
// Copyright (c) Gobie74. All rights reserved.
// </copyright>

namespace Gobie74.AspNetCore.Identity.AzureTable.Contracts
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Contract for the role class.
    /// </summary>
    [ContractClassFor(typeof(Role))]
    internal abstract class RoleContract : Role
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoleContract"/> class.
        /// </summary>
        public RoleContract()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleContract"/> class.
        /// </summary>
        /// <param name="name">The name of the role.</param>
        public RoleContract(string name)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(name));

            Contract.Ensures(this.Name == name);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleContract"/> class.
        /// </summary>
        /// <param name="id">The id of the role.</param>
        public RoleContract(Guid id)
        {
            Contract.Requires(id != Guid.Empty);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleContract"/> class.
        /// </summary>
        /// <param name="id">The id of the role.</param>
        /// <param name="roleName">The name of the role.</param>
        public RoleContract(Guid id, string roleName)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(roleName));
            Contract.Requires(id != Guid.Empty);

            Contract.Ensures(this.Name == roleName);
        }

        /// <summary>
        /// Gets or sets the role name.
        /// </summary>
        public new string Name
        {
            get
            {
                return null;
            }

            set
            {
                Contract.Requires(!string.IsNullOrWhiteSpace(value));
            }
        }
    }
}
