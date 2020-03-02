// <copyright file="UserStore.cs" company="Gobie74">
// Copyright (c) Gobie74. All rights reserved.
// </copyright>

namespace Gobie74.AspNetCore.Identity.AzureTable
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;

    using Gobie74.AzureStorage;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// User store implementation.
    /// </summary>
    public class UserStore :
        IUserStore<User>,
        IUserRoleStore<User>,
        IUserClaimStore<User>,
        IUserPasswordStore<User>,
        IUserSecurityStampStore<User>,
        IUserEmailStore<User>,
        IUserPhoneNumberStore<User>,
        //// IQueryableUserStore
        //// IUserLoginStore<User>,
        IUserTwoFactorStore<User>,
        IUserLockoutStore<User>,
        IUserAuthenticatorKeyStore<User>,
        IUserTwoFactorRecoveryCodeStore<User>
    {
        private readonly UserTableAccess userAccess;

        private readonly Lazy<RoleTableAccess> roleAccess;

        private readonly Lazy<UserInRoleTableAccess> userInRoleTableAccess;

        private readonly Lazy<UserRecoveryCodeTableAccess> userRecoveryCodeTableAccess;

        /// <summary>Initializes a new instance of the <see cref="UserStore"/> class.</summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="tableName">The default table name.</param>
        /// <param name="logger">An optional logger.</param>
        public UserStore(string connectionString, string tableName = "identity", ILogger<UserStore> logger = null)
        {
            this.Logger = logger;

            this.userAccess = new UserTableAccess(connectionString, tableName);

            this.roleAccess = new Lazy<RoleTableAccess>(() => new RoleTableAccess(connectionString, "role"));

            this.userInRoleTableAccess = new Lazy<UserInRoleTableAccess>(() => new UserInRoleTableAccess(connectionString, "role"));

            this.userRecoveryCodeTableAccess = new Lazy<UserRecoveryCodeTableAccess>(() => new UserRecoveryCodeTableAccess(connectionString, "recoverycodes"));
        }

        private ILogger<UserStore> Logger { get; }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        /// <filterpriority>2.</filterpriority>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets the user identifier for the specified <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user whose identifier should be retrieved.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the identifier for the specified <paramref name="user" />.</returns>
        public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id.ToString());
        }

        /// <summary>
        /// Gets the user name for the specified <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user whose name should be retrieved.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the name for the specified <paramref name="user" />.</returns>
        public Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        /// <summary>
        /// Sets the given <paramref name="userName" /> for the specified <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user whose name should be set.</param>
        /// <param name="userName">The user name to set.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;

            return Task.FromResult(0);
        }

        /// <summary>
        /// Gets the normalized user name for the specified <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user whose normalized name should be retrieved.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the normalized user name for the specified <paramref name="user" />.</returns>
        public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserNameNormalized);
        }

        /// <summary>
        /// Sets the given normalized name for the specified <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user whose name should be set.</param>
        /// <param name="normalizedName">The normalized name to set.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
        {
            user.UserNameNormalized = normalizedName;

            return Task.FromResult(0);
        }

        /// <summary>
        /// Creates the specified <paramref name="user" /> in the user store.
        /// </summary>
        /// <param name="user">The user to create.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the <see cref="T:Microsoft.AspNetCore.Identity.IdentityResult" /> of the creation operation.</returns>
        public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            IdentityResult result;

            try
            {
                await this.userAccess.AddAsync(user);

                result = IdentityResult.Success;
            }
            catch (StorageException e)
            {
                IdentityError error = new IdentityError { Code = "Error", Description = e.Message };

                result = IdentityResult.Failed(error);
            }

            return result;
        }

        /// <summary>
        /// Updates the specified <paramref name="user" /> in the user store.
        /// </summary>
        /// <param name="user">The user to update.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the <see cref="T:Microsoft.AspNetCore.Identity.IdentityResult" /> of the update operation.</returns>
        public async Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            IdentityResult result;

            try
            {
                await this.userAccess.InsertOrReplaceAsync(user);
                result = IdentityResult.Success;
            }
            catch (StorageException ex)
            {
                result = IdentityResult.Failed(new IdentityError { Description = ex.Message });
            }

            return result;
        }

        /// <summary>
        /// Deletes the specified <paramref name="user" /> from the user store.
        /// </summary>
        /// <param name="user">The user to delete.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the <see cref="T:Microsoft.AspNetCore.Identity.IdentityResult" /> of the update operation.</returns>
        public async Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            IdentityResult result;

            try
            {
                await this.userAccess.DeleteAsync(user);
                result = IdentityResult.Success;
            }
            catch (Exception ex)
            {
                result = IdentityResult.Failed(new IdentityError { Description = ex.Message });
            }

            return result;
        }

        /// <summary>
        /// Finds and returns a user, if any, who has the specified <paramref name="userId" />.
        /// </summary>
        /// <param name="userId">The user ID to search for.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the user matching the specified <paramref name="userId" /> if it exists.
        /// </returns>
        public async Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            try
            {
                User user = await this.userAccess.GetSingleAsync(userId, User.UserDataRowKey);

                return user;
            }
            catch (NotFoundException)
            {
                return null;
            }
        }

        /// <summary>
        /// Finds and returns a user, if any, who has the specified normalized user name.
        /// </summary>
        /// <param name="normalizedUserName">The normalized user name to search for.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the user matching the specified <paramref name="normalizedUserName" /> if it exists.
        /// </returns>
        public async Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            User user = await this.userAccess.FindFirstRowWithProperty(User.UserDataRowKey, "UserNameNormalized", normalizedUserName);

            return user;
        }

        /// <summary>
        /// Sets the password hash for the specified <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user whose password hash to set.</param>
        /// <param name="passwordHash">The password hash to set.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task" /> that represents the asynchronous operation.</returns>
        public Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;

            return Task.FromResult(0);
        }

        /// <summary>
        /// Gets the password hash for the specified <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user whose password hash to retrieve.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task" /> that represents the asynchronous operation, returning the password hash for the specified <paramref name="user" />.</returns>
        public Task<string> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        /// <summary>
        /// Gets a flag indicating whether the specified <paramref name="user" /> has a password.
        /// </summary>
        /// <param name="user">The user to return a flag for, indicating whether they have a password or not.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task" /> that represents the asynchronous operation, returning true if the specified <paramref name="user" /> has a password
        /// otherwise false.
        /// </returns>
        public Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(!string.IsNullOrWhiteSpace(user.PasswordHash));
        }

        /// <summary>
        /// Gets the last <see cref="DateTimeOffset" /> a user's last lockout expired, if any.
        /// Any time in the past should be indicates a user is not locked out.
        /// </summary>
        /// <param name="user">The user whose lockout date should be retrieved.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// A <see cref="Task{TResult}" /> that represents the result of the asynchronous query, a <see cref="DateTimeOffset" /> containing the last time
        /// a user's lockout expired, if any.
        /// </returns>
        public Task<DateTimeOffset?> GetLockoutEndDateAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.LockoutEndDate);
        }

        /// <summary>
        /// Locks out a user until the specified end date has passed. Setting a end date in the past immediately unlocks a user.
        /// </summary>
        /// <param name="user">The user whose lockout date should be set.</param>
        /// <param name="lockoutEnd">The <see cref="DateTimeOffset" /> after which the <paramref name="user" />'s lockout should end.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task" /> that represents the asynchronous operation.</returns>
        public Task SetLockoutEndDateAsync(User user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
        {
            user.LockoutEndDate = lockoutEnd;
            return Task.FromResult(0);
        }

        /// <summary>
        /// Records that a failed access has occurred, incrementing the failed access count.
        /// </summary>
        /// <param name="user">The user whose cancellation count should be incremented.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task" /> that represents the asynchronous operation, containing the incremented failed access count.</returns>
        public Task<int> IncrementAccessFailedCountAsync(User user, CancellationToken cancellationToken)
        {
            user.FailedAccessCount++;

            return Task.FromResult(user.FailedAccessCount);
        }

        /// <summary>
        /// Resets a user's failed access count.
        /// </summary>
        /// <param name="user">The user whose failed access count should be reset.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task" /> that represents the asynchronous operation.</returns>
        /// <remarks>This is typically called after the account is successfully accessed.</remarks>
        public Task ResetAccessFailedCountAsync(User user, CancellationToken cancellationToken)
        {
            user.FailedAccessCount = 0;

            return Task.FromResult(0);
        }

        /// <summary>
        /// Retrieves the current failed access count for the specified <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user whose failed access count should be retrieved.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task" /> that represents the asynchronous operation, containing the failed access count.</returns>
        public Task<int> GetAccessFailedCountAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.FailedAccessCount);
        }

        /// <summary>
        /// Retrieves a flag indicating whether user lockout can enabled for the specified user.
        /// </summary>
        /// <param name="user">The user whose ability to be locked out should be returned.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task" /> that represents the asynchronous operation, true if a user can be locked out, otherwise false.
        /// </returns>
        public Task<bool> GetLockoutEnabledAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.CanBeLockedOut);
        }

        /// <summary>
        /// Set the flag indicating if the specified <paramref name="user" /> can be locked out.
        /// </summary>
        /// <param name="user">The user whose ability to be locked out should be set.</param>
        /// <param name="enabled">A flag indicating if lock out can be enabled for the specified <paramref name="user" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task" /> that represents the asynchronous operation.</returns>
        public Task SetLockoutEnabledAsync(User user, bool enabled, CancellationToken cancellationToken)
        {
            user.CanBeLockedOut = enabled;
            return Task.FromResult(0);
        }

        /// <summary>
        /// Sets the <paramref name="email" /> address for a <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user whose email should be set.</param>
        /// <param name="email">The email to set.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public Task SetEmailAsync(User user, string email, CancellationToken cancellationToken)
        {
            user.Email = email;

            return Task.FromResult(0);
        }

        /// <summary>
        /// Gets the email address for the specified <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user whose email should be returned.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The task object containing the results of the asynchronous operation, the email address for the specified <paramref name="user" />.</returns>
        public Task<string> GetEmailAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email);
        }

        /// <summary>
        /// Gets a flag indicating whether the email address for the specified <paramref name="user" /> has been verified, true if the email address is verified otherwise
        /// false.
        /// </summary>
        /// <param name="user">The user whose email confirmation status should be returned.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object containing the results of the asynchronous operation, a flag indicating whether the email address for the specified <paramref name="user" />
        /// has been confirmed or not.
        /// </returns>
        public Task<bool> GetEmailConfirmedAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.EmailConfirmed);
        }

        /// <summary>
        /// Sets the flag indicating whether the specified <paramref name="user" />'s email address has been confirmed or not.
        /// </summary>
        /// <param name="user">The user whose email confirmation status should be set.</param>
        /// <param name="confirmed">A flag indicating if the email address has been confirmed, true if the address is confirmed otherwise false.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public Task SetEmailConfirmedAsync(User user, bool confirmed, CancellationToken cancellationToken)
        {
            user.EmailConfirmed = confirmed;

            return Task.FromResult(0);
        }

        /// <summary>
        /// Gets the user, if any, associated with the specified, normalized email address.
        /// </summary>
        /// <param name="normalizedEmail">The normalized email address to return the user for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object containing the results of the asynchronous lookup operation, the user if any associated with the specified normalized email address.
        /// </returns>
        public async Task<User> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            try
            {
                User found = await this.userAccess.FindFirstRowWithProperty(User.UserDataRowKey, "NormalizedEmail", normalizedEmail);

                return found;
            }
            catch (Exception e)
            {
                this.Logger?.LogError($"Error while finding user by email {normalizedEmail}.", e);
                return null;
            }
        }

        /// <summary>
        /// Returns the normalized email for the specified <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user whose email address to retrieve.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object containing the results of the asynchronous lookup operation, the normalized email address if any associated with the specified user.
        /// </returns>
        public Task<string> GetNormalizedEmailAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedEmail);
        }

        /// <summary>
        /// Sets the normalized email for the specified <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user whose email address to set.</param>
        /// <param name="normalizedEmail">The normalized email to set for the specified <paramref name="user" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public Task SetNormalizedEmailAsync(User user, string normalizedEmail, CancellationToken cancellationToken)
        {
            user.NormalizedEmail = normalizedEmail;

            return Task.FromResult(0);
        }

        /// <summary>
        /// Sets the telephone number for the specified <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user whose telephone number should be set.</param>
        /// <param name="phoneNumber">The telephone number to set.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task" /> that represents the asynchronous operation.</returns>
        public Task SetPhoneNumberAsync(User user, string phoneNumber, CancellationToken cancellationToken)
        {
            user.PhoneNumber = phoneNumber;

            return Task.FromResult(0);
        }

        /// <summary>
        /// Gets the telephone number, if any, for the specified <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user whose telephone number should be retrieved.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task" /> that represents the asynchronous operation, containing the user's telephone number, if any.</returns>
        public Task<string> GetPhoneNumberAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PhoneNumber);
        }

        /// <summary>
        /// Gets a flag indicating whether the specified <paramref name="user" />'s telephone number has been confirmed.
        /// </summary>
        /// <param name="user">The user to return a flag for, indicating whether their telephone number is confirmed.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task" /> that represents the asynchronous operation, returning true if the specified <paramref name="user" /> has a confirmed
        /// telephone number otherwise false.
        /// </returns>
        public Task<bool> GetPhoneNumberConfirmedAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        /// <summary>
        /// Sets a flag indicating if the specified <paramref name="user" />'s phone number has been confirmed.
        /// </summary>
        /// <param name="user">The user whose telephone number confirmation status should be set.</param>
        /// <param name="confirmed">A flag indicating whether the user's telephone number has been confirmed.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task" /> that represents the asynchronous operation.</returns>
        public Task SetPhoneNumberConfirmedAsync(User user, bool confirmed, CancellationToken cancellationToken)
        {
            user.PhoneNumberConfirmed = confirmed;

            return Task.FromResult(0);
        }

        /// <summary>
        /// Add the specified <paramref name="user" /> to the named role.
        /// </summary>
        /// <param name="user">The user to add to the named role.</param>
        /// <param name="roleName">The name of the role to add the user to.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task" /> that represents the asynchronous operation.</returns>
        public async Task AddToRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            // 1. Find the RoleId
            Role requestedRole = await this.roleAccess.Value.FindFirstRowWithProperty(Role.RowKeyIdentifier, "Name", roleName);

            if (requestedRole == null)
            {
                return;
            }

            // 2. Check if the user already has the role
            UserInRole existingRole = await this.userInRoleTableAccess.Value.GetSingleAsync(requestedRole.Id.ToString(), user.Id.ToString());

            if (existingRole != null)
            {
                return;
            }

            // 3. Add user to role
            UserInRole newRole = new UserInRole(requestedRole.Id, user.Id) { AddedUtc = DateTime.UtcNow, AddedBy = "system?" };

            await this.userInRoleTableAccess.Value.AddAsync(newRole);
        }

        /// <summary>
        /// Remove the specified <paramref name="user" /> from the named role.
        /// </summary>
        /// <param name="user">The user to remove the named role from.</param>
        /// <param name="roleName">The name of the role to remove.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task" /> that represents the asynchronous operation.</returns>
        public async Task RemoveFromRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            // 1. Find the RoleId
            Role requestedRole = await this.roleAccess.Value.FindFirstRowWithProperty(Role.RowKeyIdentifier, "Name", roleName);

            if (requestedRole == null)
            {
                return;
            }

            // 2. Check if the user already has the role
            UserInRole existingRole = await this.userInRoleTableAccess.Value.GetSingleAsync(requestedRole.Id.ToString(), user.Id.ToString());

            if (existingRole == null)
            {
                return;
            }

            // 3. Remove the user from the role
            await this.userInRoleTableAccess.Value.DeleteAsync(existingRole);
        }

        /// <summary>
        /// Gets a list of role names the specified <paramref name="user" /> belongs to.
        /// </summary>
        /// <param name="user">The user whose role names to retrieve.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task" /> that represents the asynchronous operation, containing a list of role names.</returns>
        public async Task<IList<string>> GetRolesAsync(User user, CancellationToken cancellationToken)
        {
            List<string> roleNames = new List<string>();

            List<Guid> roles = null;
            try
            {
                IReadOnlyCollection<UserInRole> userInRoles = await this.userInRoleTableAccess.Value.FindAllByRowKey(user.Id.ToString());

                roles = userInRoles.Select(x => x.Role).ToList();
            }
            catch (StorageException)
            {
                return roleNames;
            }

            foreach (var roleId in roles)
            {
                try
                {
                    Role theRole = await this.roleAccess.Value.GetSingleAsync(roleId.ToString(), Role.RowKeyIdentifier);

                    if (theRole != null)
                    {
                        roleNames.Add(theRole.Name);
                    }
                }
                catch (StorageException)
                {
                    continue;
                }
            }

            return roleNames;
        }

        /// <summary>
        /// Returns a flag indicating whether the specified <paramref name="user" /> is a member of the given named role.
        /// </summary>
        /// <param name="user">The user whose role membership should be checked.</param>
        /// <param name="roleName">The name of the role to be checked.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task" /> that represents the asynchronous operation, containing a flag indicating whether the specified <paramref name="user" /> is
        /// a member of the named role.
        /// </returns>
        public async Task<bool> IsInRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            // 1. Find the RoleId
            Role requestedRole = await this.roleAccess.Value.FindFirstRowWithProperty(Role.RowKeyIdentifier, "Name", roleName);

            if (requestedRole == null)
            {
                return false;
            }

            // 2. Check if the user already has the role
            UserInRole existingRole = await this.userInRoleTableAccess.Value.GetSingleAsync(requestedRole.Id.ToString(), user.Id.ToString());

            // 3. If the user has the role, return true.
            return existingRole != null;
        }

        /// <summary>
        /// Returns a list of Users who are members of the named role.
        /// </summary>
        /// <param name="roleName">The name of the role whose membership should be returned.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task" /> that represents the asynchronous operation, containing a list of users who are in the named role.
        /// </returns>
        public async Task<IList<User>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            // TODO : Finish
            IList<User> users = new List<User>();

            // 1. Get the role id for the given role name.
            Role role = await this.roleAccess.Value.FindFirstRowWithProperty(Role.RowKeyIdentifier, "Name", roleName);

            if (role == null)
            {
                return users;
            }

            // 2. Find all the UserInRole records for the role id
            IReadOnlyCollection<UserInRole> usersInRole = await this.userInRoleTableAccess.Value.FindAllByPartitionKey(role.Id.ToString());

            foreach (var userInRole in usersInRole)
            {
                User user = await this.userAccess.GetSingleAsync(userInRole.User.ToString(), User.UserDataRowKey);

                if (user != null)
                {
                    users.Add(user);
                }
            }

            // 3. Get User records for all the user in role records
            return users;
        }

        /// <summary>
        /// Sets a flag indicating whether the specified <paramref name="user" /> has two factor authentication enabled or not,
        /// as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user whose two factor authentication enabled status should be set.</param>
        /// <param name="enabled">A flag indicating whether the specified <paramref name="user" /> has two factor authentication enabled.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task" /> that represents the asynchronous operation.</returns>
        public Task SetTwoFactorEnabledAsync(User user, bool enabled, CancellationToken cancellationToken)
        {
            user.TwoFactorEnabled = enabled;

            return Task.CompletedTask;
        }

        /// <summary>
        /// Returns a flag indicating whether the specified <paramref name="user" /> has two factor authentication enabled or not,
        /// as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user whose two factor authentication enabled status should be set.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task" /> that represents the asynchronous operation, containing a flag indicating whether the specified
        /// <paramref name="user" /> has two factor authentication enabled or not.
        /// </returns>
        public Task<bool> GetTwoFactorEnabledAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult<bool>(user.TwoFactorEnabled);
        }

        /// <summary>
        /// Sets the provided security <paramref name="stamp" /> for the specified <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user whose security stamp should be set.</param>
        /// <param name="stamp">The security stamp to set.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task" /> that represents the asynchronous operation.</returns>
        public Task SetSecurityStampAsync(User user, string stamp, CancellationToken cancellationToken)
        {
            user.SecurityStamp = stamp;

            return Task.CompletedTask;
        }

        /// <summary>
        /// Get the security stamp for the specified <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user whose security stamp should be set.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task" /> that represents the asynchronous operation, containing the security stamp for the specified <paramref name="user" />.</returns>
        public Task<string> GetSecurityStampAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult<string>(user.SecurityStamp);
        }

        /// <summary>
        /// Gets a list of <see cref="Claim" />s to be belonging to the specified <paramref name="user" /> as an asynchronous operation.
        /// </summary>
        /// <param name="user">The role whose claims to retrieve.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// A <see cref="Task{TResult}" /> that represents the result of the asynchronous query, a list of <see cref="Claim" />s.
        /// </returns>
        public Task<IList<Claim>> GetClaimsAsync(User user, CancellationToken cancellationToken)
        {
            // TODO : reimplement correctly.
            IList<Claim> claims = new List<Claim>();
            claims.Add(new Claim("hello", "world"));

            return Task.FromResult(claims);
        }

        /// <summary>
        /// Add claims to a user as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user to add the claim to.</param>
        /// <param name="claims">The collection of <see cref="Claim" />s to add.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public Task AddClaimsAsync(User user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Replaces the given <paramref name="claim" /> on the specified <paramref name="user" /> with the <paramref name="newClaim" />.
        /// </summary>
        /// <param name="user">The user to replace the claim on.</param>
        /// <param name="claim">The claim to replace.</param>
        /// <param name="newClaim">The new claim to replace the existing <paramref name="claim" /> with.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public Task ReplaceClaimAsync(User user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Removes the specified <paramref name="claims" /> from the given <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user to remove the specified <paramref name="claims" /> from.</param>
        /// <param name="claims">A collection of <see cref="Claim" />s to remove.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public Task RemoveClaimsAsync(User user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Returns a list of users who contain the specified <see cref="Claim" />.
        /// </summary>
        /// <param name="claim">The claim to look for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks.Task{TResult}" /> that represents the result of the asynchronous query, a list of Users who
        /// contain the specified claim.
        /// </returns>
        public async Task<IList<User>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            var users = await this.userAccess.FindAllByRowKey("UserInfo");

            return new ReadOnlyCollection<User>(users.ToList());
        }

        /// <summary>
        /// Sets the authenticator key for the specified <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user whose authenticator key should be set.</param>
        /// <param name="key">The authenticator key to set.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task" /> that represents the asynchronous operation.</returns>
        public Task SetAuthenticatorKeyAsync(User user, string key, CancellationToken cancellationToken)
        {
            user.AuthenticatorKey = key;

            return Task.CompletedTask;
        }

        /// <summary>
        /// Get the authenticator key for the specified <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user whose security stamp should be set.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task" /> that represents the asynchronous operation, containing the security stamp for the specified <paramref name="user" />.</returns>
        public Task<string> GetAuthenticatorKeyAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult<string>(user.AuthenticatorKey);
        }

        /// <summary>
        /// Updates the recovery codes for the user while invalidating any previous recovery codes.
        /// </summary>
        /// <param name="user">The user to store new recovery codes for.</param>
        /// <param name="recoveryCodes">The new recovery codes for the user.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task ReplaceCodesAsync(User user, IEnumerable<string> recoveryCodes, CancellationToken cancellationToken)
        {
            var existingCodes = await this.userRecoveryCodeTableAccess.Value.FindAllByPartitionKey(user.PartitionKey);

            foreach (var code in existingCodes)
            {
                await this.userRecoveryCodeTableAccess.Value.DeleteAsync(code);
            }

            foreach (string newCode in recoveryCodes)
            {
                UserRecoveryCode recoveryCode = new UserRecoveryCode(user.PartitionKey, newCode) { AddedOn = DateTime.Now };

                await this.userRecoveryCodeTableAccess.Value.AddAsync(recoveryCode);
            }
        }

        /// <summary>
        /// Returns whether a recovery code is valid for a user. Note: recovery codes are only valid
        /// once, and will be invalid after use.
        /// </summary>
        /// <param name="user">The user who owns the recovery code.</param>
        /// <param name="code">The recovery code to use.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>True if the recovery code was found for the user.</returns>
        public async Task<bool> RedeemCodeAsync(User user, string code, CancellationToken cancellationToken)
        {
            UserRecoveryCode existingCode;

            try
            {
                existingCode = await this.userRecoveryCodeTableAccess.Value.GetSingleAsync(user.PartitionKey, code);

                if (existingCode == null)
                {
                    return false;
                }
            }
            catch (NotFoundException)
            {
                return false;
            }

            await this.userRecoveryCodeTableAccess.Value.DeleteAsync(existingCode);

            return true;
        }

        /// <summary>
        /// Returns how many recovery code are still valid for a user.
        /// </summary>
        /// <param name="user">The user who owns the recovery code.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The number of valid recovery codes for the user.</returns>
        public async Task<int> CountCodesAsync(User user, CancellationToken cancellationToken)
        {
            try
            {
                var codesForUser = await this.userRecoveryCodeTableAccess.Value.FindAllByPartitionKey(user.PartitionKey);

                if (codesForUser != null)
                {
                    return codesForUser.Count();
                }
                else
                {
                    return 0;
                }
            }
            catch (NotFoundException)
            {
                return 0;
            }
        }

        /// <summary>
        /// Dispose implementation.
        /// </summary>
        /// <param name="disposing">true if disposing, false if in deconstruction.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.userAccess.Dispose();
            }
        }
    }
}
