using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Novell.Directory.Ldap;
using getnet.core.Model;
using getnet.core.Model.Entities;
using System.Security.Claims;

namespace getnet.Model.Security
{
    public class EveryonesAnAdminProvider : SecurityProvider
    {

        public override Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
        {
            return new Task<string>(() => user.AccountName);
        }

        public override Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return new Task<string>(() => user.AccountName);
        }

        public override Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return new Task<string>(() => user.AccountName);
        }

        public override Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            return FindUserAsync(userId, cancellationToken);
        }

        public override Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            return FindUserAsync(normalizedUserName, cancellationToken);
        }

        public Task<User> FindUserAsync(string email, CancellationToken cancellationToken)
        {
            return Task.FromResult(new Model.User("AdminUser@GetNet", Roles.GlobalAdmins));
        }

        public override void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~EveryonesAnAdminProvider()
        {
            Dispose(false);
        }

        public void Dispose(bool disposing)
        {
            if (disposing)
            {
                //things
            }
        }

        public override Task<string> GetRoleIdAsync(Role role, CancellationToken cancellationToken)
        {
            return new Task<string>(() => role.Name);
        }

        public override Task<string> GetRoleNameAsync(Role role, CancellationToken cancellationToken)
        {
            return new Task<string>(() => role.Name);
        }

        public override Task<string> GetNormalizedRoleNameAsync(Role role, CancellationToken cancellationToken)
        {
            return new Task<string>(() => role.Name);
        }

        public override Task<Role> FindRoleByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            return new Task<Role>(() =>
            {
                return new Role() { Name = roleId };
            });
        }

        public override Task<IList<string>> GetRolesAsync(User user, CancellationToken cancellationToken)
        {
            return new Task<IList<string>>(() =>
            {
                return user.Claims.Where(d => d.Type == ClaimTypes.Role).Select(d => d.Value).ToList();
            });
        }

        public override Task<bool> IsInRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        public override Task<IList<User>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            return Task.FromResult(new List<User> { new Model.User("AdminUser@GetNet", Roles.GlobalAdmins) } as IList<User>);
        }

        public override Task<ClaimsPrincipal> CreateAsync(User user)
        {
            return Task.FromResult((ClaimsPrincipal)user);
        }
    }
}