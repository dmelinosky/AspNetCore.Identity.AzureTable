// <copyright file="RoleStore.cs" company="Gobie74">
// Copyright (c) Gobie74. All rights reserved.
// </copyright>

namespace Gobie74.AspNetCore.Identity.AzureTable
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Gobie74.AzureStorage;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Role store implemenation.
    /// </summary>
    public class RoleStore : IRoleStore<Role>
    {
        /// <summary> table access. </summary>
        private readonly RoleTableAccess roleAccess;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleStore"/> class.
        /// </summary>
        /// <param name="connectionString">the connection string.</param>
        /// <param name="tableName">the table name.</param>
        /// <param name="logger">an optional logger.</param>
        public RoleStore(string connectionString, string tableName = "role", ILogger<RoleStore> logger = null)
        {
            this.Logger = logger;

            this.Logger?.LogTrace($"Constructing RoleStore with connection string {connectionString} and table named {tableName}.");
            this.roleAccess = new RoleTableAccess(connectionString, tableName);
            this.Logger?.LogTrace("RoleStore construction complete.");
        }

        private ILogger<RoleStore> Logger { get; }

        /// <inheritdoc/>
        public async Task<IdentityResult> CreateAsync(Role role, CancellationToken cancellationToken)
        {
            IdentityResult result;
            try
            {
                this.Logger?.LogTrace("Sending role for creation.");

                await this.roleAccess.AddAsync(role);

                result = IdentityResult.Success;
            }
            catch (StorageException e)
            {
                this.Logger?.LogError("Role creation error, exception caught", e);

                IdentityError error = new IdentityError { Code = "Error", Description = e.Message };

                result = IdentityResult.Failed(error);
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<IdentityResult> DeleteAsync(Role role, CancellationToken cancellationToken)
        {
            IdentityResult result;
            try
            {
                await this.roleAccess.DeleteAsync(role);

                result = IdentityResult.Success;
            }
            catch (StorageException ex)
            {
                this.Logger?.LogError(ex.Message, ex);
                result = IdentityResult.Failed(new IdentityError { Description = ex.Message });
            }

            return result;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        public async Task<Role> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            try
            {
                Role role = await this.roleAccess.GetSingleAsync(roleId, Role.RowKeyIdentifier);

                return role;
            }
            catch (NotFoundException)
            {
                return null;
            }
        }

        /// <inheritdoc/>
        public async Task<Role> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            try
            {
                Role role = await this.roleAccess.FindFirstRowWithProperty(Role.RowKeyIdentifier, "NormalizedName", normalizedRoleName);

                return role;
            }
            catch (NotFoundException)
            {
                return null;
            }
        }

        /// <inheritdoc/>
        public Task<string> GetNormalizedRoleNameAsync(Role role, CancellationToken cancellationToken)
        {
            return Task.FromResult<string>(role.NormalizedName);
        }

        /// <inheritdoc/>
        public Task<string> GetRoleIdAsync(Role role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Id.ToString());
        }

        /// <inheritdoc/>
        public Task<string> GetRoleNameAsync(Role role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Name);
        }

        /// <inheritdoc/>
        public Task SetNormalizedRoleNameAsync(Role role, string normalizedName, CancellationToken cancellationToken)
        {
            role.NormalizedName = normalizedName;

            return Task.FromResult(0);
        }

        /// <inheritdoc/>
        public Task SetRoleNameAsync(Role role, string roleName, CancellationToken cancellationToken)
        {
            role.Name = roleName;

            return Task.FromResult(0);
        }

        /// <inheritdoc/>
        public async Task<IdentityResult> UpdateAsync(Role role, CancellationToken cancellationToken)
        {
            IdentityResult result;
            try
            {
                await this.roleAccess.InsertOrReplaceAsync(role);

                result = IdentityResult.Success;
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(ex.Message, ex);
                result = IdentityResult.Failed(new IdentityError { Description = ex.Message });
            }

            return result;
        }

        /// <summary>
        /// Dispose implementation.
        /// </summary>
        /// <param name="disposing">true if disposing, false if in deconstruction.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.roleAccess.Dispose();
            }
        }
    }
}
