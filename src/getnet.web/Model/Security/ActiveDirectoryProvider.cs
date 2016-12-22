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
    public class ActiveDirectoryProvider : SecurityProvider
    {

        protected UnitOfWork uow;

        public ActiveDirectoryProvider() : this(new UnitOfWork())
        {
        }

        public ActiveDirectoryProvider(UnitOfWork unitOfWork)
        {
            uow = unitOfWork;
        }

        public override bool InGroups(string groupNames, string accountName)
        {
            foreach (var group in groupNames.Split(';'))
            {
                if (LdapServer.Current.InGroup(accountName, group.Trim()))
                    return true;
            }
            return false;
        }

        public override bool ValidateUser(string email, string password)
        {
            return LdapServer.Current.Authenticate(email, password);
        }

        public Task<User> FindUserAsync(string email, CancellationToken cancellationToken)
        {
            return new Task<User>(() =>
            {
                if (cancellationToken.IsCancellationRequested)
                    return null;

                var entry = Retry.Do(() => LdapServer.Current.FindUser(email), TimeSpan.FromSeconds(1));
                var profile = uow.Repo<UserProfile>().Get(d => d.Email == email).FirstOrDefault();
                var user = new User(entry.getAttribute("mail").StringValue, profile);
                return user;
            });
        }

        public override Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            return FindUserAsync(userId, cancellationToken);
        }

        public override Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            return FindUserAsync(normalizedUserName, cancellationToken);
        }

        public override Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return new Task<string>(() => user.AccountName);
        }

        public override Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
        {
            return new Task<string>(() => user.AccountName);
        }

        public override Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return new Task<string>(() => user.AccountName);
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
                return user.Roles.Select(d => d.RoleId).ToList();
            });
        }

        public override Task<IList<User>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override Task<bool> IsInRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            return new Task<bool>(() =>
            {
                return Retry.Do(() => LdapServer.Current.InGroup(user.AccountName, roleName), TimeSpan.FromSeconds(1));
            });
        }

        public override Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user, string password)
        {
            return new Task<IdentityResult>(() =>
            {
                var signInResult = false;
                Exception lastException = null;
                try
                {
                    signInResult = Retry.Do(() => LdapServer.Current.Authenticate(user.AccountName, password), TimeSpan.FromSeconds(1));
                } catch (AggregateException ex)
                {
                    lastException = ex.InnerExceptions.Last();
                }
                var result = new AuthenticateResult(signInResult, lastException?.Message ?? null);
                return (IdentityResult)result;
                
            });
        }


        public override void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ActiveDirectoryProvider()
        {
            Dispose(false);
        }

        public void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (uow != null)
                {
                    uow.Dispose();
                    uow = null;
                }
            }
        }

        private List<User> _logins = new List<User>();

        public override Task AddLoginAsync(User user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            user.Logins.Add(new Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>
            {
                UserId = user.AccountName,
                LoginProvider = login.LoginProvider,
                ProviderDisplayName = login.ProviderDisplayName,
                ProviderKey = login.ProviderKey
            });
            _logins.Add(user);
            return Task.FromResult(0);
        }

        public override Task RemoveLoginAsync(User user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            _logins.Remove(user);
            return Task.FromResult(0);
        }

        public override Task<IList<UserLoginInfo>> GetLoginsAsync(User user, CancellationToken cancellationToken)
        {
            IList<UserLoginInfo> logins = user.Logins.Select(d => new UserLoginInfo(d.LoginProvider, d.ProviderKey, d.ProviderDisplayName)).ToList();
            return Task.FromResult(logins);
        }

        public override Task<User> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            var login = _logins.FirstOrDefault(d => d.Logins.Any(l => l.LoginProvider == loginProvider) && d.Logins.Any(l => l.ProviderKey == providerKey));
            return Task.FromResult(login);
        }

        public override Task<ClaimsPrincipal> CreateAsync(User user)
        {
            return Task.FromResult(user.CreatePrincipal());
        }
    }
}