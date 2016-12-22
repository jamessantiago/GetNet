using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace getnet.Model.Security
{
    public abstract class SecurityProvider : IUserStore<User>, IRoleStore<Role>, IUserRoleStore<User>, IPasswordValidator<User>, IUserPasswordStore<User>, IUserLoginStore<User>, IUserClaimsPrincipalFactory<User>
    {
        public virtual bool IsAdmin => InGroups(CoreCurrent.Configuration["Security:GlobalAdminGroups"]);
        public virtual bool IsViewer => InGroups(CoreCurrent.Configuration["Security:GlobalViewGroups"]);

        internal virtual bool InReadGroups(ISecurableModule settings)
        {
            return IsViewer || (settings != null && (InGroups(settings.ViewGroups) || InAdminGroups(settings)));
        }

        internal virtual bool InAdminGroups(ISecurableModule settings)
        {
            return IsAdmin || (settings != null && InGroups(settings.AdminGroups));
        }

        private bool InGroups(string groupNames)
        {
            return true;
            //if (groupNames.IsNullOrEmpty() || Current.User.AccountName.IsNullOrEmpty()) return false;
            //return groupNames == "*" || InGroups(groupNames, Current.User.AccountName);
        }

        public abstract bool InGroups(string groupNames, string accountName);
        public abstract bool ValidateUser(string userName, string password);

        public virtual List<string> GetGroupMembers(string groupName)
        {
            return new List<string>();
        }
        public virtual void PurgeCache() { }

        public List<IPNetwork> InternalNetworks;
        protected SecurityProvider()
        {
            InternalNetworks = new List<IPNetwork>();
            if (CoreCurrent.Configuration["Security:InternalNetworks"].HasValue())
            {
                foreach (var n in CoreCurrent.Configuration["Security:InternalNetworks"].Split(';'))
                {
                    IPNetwork net = null;
                    if (IPNetwork.TryParse(n, out net))
                    {
                        InternalNetworks.Add(net);
                    }
                }
            }
        }

        public bool IsInternalIP(string ip)
        {
            IPAddress addr;
            return IPAddress.TryParse(ip, out addr) && InternalNetworks.Any(n => IPNetwork.Contains(n,  addr));
        }

        public abstract Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken);
        public abstract Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken);
        
        public abstract Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken);
        public abstract Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken);
        public abstract Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken);
        public abstract void Dispose();
        
        public abstract Task<string> GetRoleIdAsync(Role role, CancellationToken cancellationToken);
        public abstract Task<string> GetRoleNameAsync(Role role, CancellationToken cancellationToken);
        public abstract Task<string> GetNormalizedRoleNameAsync(Role role, CancellationToken cancellationToken);
        
        public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> CreateAsync(Role role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> UpdateAsync(Role role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> DeleteAsync(Role role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetRoleNameAsync(Role role, string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetNormalizedRoleNameAsync(Role role, string normalizedName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public abstract Task<Role> FindRoleByIdAsync(string roleId, CancellationToken cancellationToken);

        Task<Role> IRoleStore<Role>.FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            return FindRoleByIdAsync(roleId, cancellationToken);
        }
        
        Task<Role> IRoleStore<Role>.FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            return FindRoleByIdAsync(normalizedRoleName, cancellationToken);
        }

        public abstract Task<IList<string>> GetRolesAsync(User user, CancellationToken cancellationToken);
        public abstract Task<bool> IsInRoleAsync(User user, string roleName, CancellationToken cancellationToken);
        public abstract Task<IList<User>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken);

        public Task AddToRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task RemoveFromRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public abstract Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user, string password);

        public Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
        {
            return new Task<string>(() => string.Empty);
        }

        public Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public abstract Task AddLoginAsync(User user, UserLoginInfo login, CancellationToken cancellationToken);
        public abstract Task RemoveLoginAsync(User user, string loginProvider, string providerKey, CancellationToken cancellationToken);
        public abstract Task<IList<UserLoginInfo>> GetLoginsAsync(User user, CancellationToken cancellationToken);
        public abstract Task<User> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken);
        public abstract Task<ClaimsPrincipal> CreateAsync(User user);
    }

    public interface ISecurableModule
    {
        bool Enabled { get; }
        string ViewGroups { get; }
        string AdminGroups { get; }
    }
}