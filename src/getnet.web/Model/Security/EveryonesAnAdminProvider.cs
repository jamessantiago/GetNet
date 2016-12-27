using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace getnet.Model.Security
{
    public class EveryonesAnAdminProvider : SecurityProvider
    {

        public override Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }

        public override Task<string> GetRoleIdAsync(Role role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override Task<string> GetRoleNameAsync(Role role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override Task<string> GetNormalizedRoleNameAsync(Role role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override Task<Role> FindRoleByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override Task<IList<string>> GetRolesAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override Task<bool> IsInRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override Task<IList<User>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override Task<ClaimsPrincipal> CreateAsync(User user)
        {
            throw new NotImplementedException();
        }
    }
}