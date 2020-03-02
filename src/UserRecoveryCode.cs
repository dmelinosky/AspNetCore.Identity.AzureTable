// <copyright file="UserRecoveryCode.cs" company="Gobie74">
// Copyright (c) Gobie74. All rights reserved.
// </copyright>

namespace Gobie74.AspNetCore.Identity.AzureTable
{
    using System;
    using Microsoft.Azure.Cosmos.Table;

    /// <summary>
    /// Class to store user recovery codes.
    /// </summary>
    /// <remarks>The partition is the user id (guid) and the row it the recovery code itself.</remarks>
    public class UserRecoveryCode : TableEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserRecoveryCode"/> class.
        /// </summary>
        public UserRecoveryCode()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRecoveryCode"/> class.
        /// </summary>
        /// <param name="user">user id.</param>
        /// <param name="code">the recovery code.</param>
        public UserRecoveryCode(Guid user, string code)
        {
            this.PartitionKey = user.ToString();
            this.RowKey = code;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRecoveryCode"/> class.
        /// </summary>
        /// <param name="user">the user id.</param>
        /// <param name="code">the recovery code.</param>
        public UserRecoveryCode(string user, string code)
        {
            this.PartitionKey = user;
            this.RowKey = code;
        }

        /// <summary>
        /// Gets or sets the date and time the code was added.
        /// </summary>
        public DateTime AddedOn { get; set; }
    }
}
