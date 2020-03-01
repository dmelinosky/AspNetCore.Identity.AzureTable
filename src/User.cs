// <copyright file="User.cs" company="Gobie74">
// Copyright (c) Gobie74. All rights reserved.
// </copyright>

namespace Gobie74.AspNetCore.Identity.AzureTable
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.Azure.Cosmos.Table;

    /// <summary>
    /// User implementation.
    /// </summary>
    public class User : TableEntity
    {
        /// <summary> The user data row key for table storage. </summary>
        public const string UserDataRowKey = "UserInfo";

        /// <summary>
        /// backing store for user name.
        /// </summary>
        private string userName;

        /// <summary> Initializes a new instance of the <see cref="User"/> class. </summary>
        public User()
        {
            this.PartitionKey = Guid.NewGuid().ToString();
            this.RowKey = UserDataRowKey;
        }

        /// <summary> Initializes a new instance of the <see cref="User"/> class. </summary>
        /// <param name="id">the user id.</param>
        public User(Guid id)
            : this()
        {
            this.Id = id;
        }

        /// <summary> Gets or sets the user id. </summary>
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

        /// <summary> Gets or sets the user name. </summary>
        public string UserName
        {
            get
            {
                return this.userName;
            }

            set
            {
                this.userName = value.Trim();
                this.UserNameNormalized = value.ToLower().Trim();
            }
        }

        /// <summary> Gets or sets the users password hash. </summary>
        [Required]
        [StringLength(100)]
        public string PasswordHash { get; set; }

        /// <summary> Gets or sets the users email address. </summary>
        [StringLength(100)]
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        /// <summary> Gets or sets a normalized email address, if available. </summary>
        [StringLength(100)]
        [EmailAddress]
        public string NormalizedEmail { get; set; }

        /// <summary> Gets or sets a value indicating whether the email address has been confirmed. </summary>
        public bool EmailConfirmed { get; set; }

        /// <summary> Gets or sets the lockout end date. </summary>
        public DateTimeOffset? LockoutEndDate { get; set; }

        /// <summary> Gets or sets the number of times the user has unsuccessfully logged in. </summary>
        public int FailedAccessCount { get; set; }

        /// <summary> Gets or sets a value indicating whether the user can be locked out. </summary>
        public bool CanBeLockedOut { get; set; }

        /// <summary> Gets or sets a security stamp. </summary>
        public string SecurityStamp { get; set; }

        /// <summary> Gets or sets a value indicating whether the user has enabled two factor authentication. </summary>
        public bool TwoFactorEnabled { get; set; }

        /// <summary> Gets or sets the users phone number. </summary>
        public string PhoneNumber { get; set; }

        /// <summary> Gets or sets a value indicating whether the user's phone number has been confirmed. </summary>
        public bool PhoneNumberConfirmed { get; set; }

        /// <summary> Gets or sets a normalized user name, user store seems to want this?.</summary>
        public string UserNameNormalized { get; set; }

        /// <summary> Gets or sets the users authenticator key. </summary>
        public string AuthenticatorKey { get; set; }

        /// <inheritdoc/>
        public override string ToString() => $"{this.Id} - {this.UserName}";
    }
}