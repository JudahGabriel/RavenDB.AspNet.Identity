using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Blun.AspNet.Identity.RavenDB.Common;
using Blun.AspNet.Identity.RavenDB.Entity;
using Blun.AspNet.Identity.RavenDB.Index;
using Microsoft.AspNet.Identity;
using Raven.Client;
using RavenDB.AspNet.Identity;

namespace Blun.AspNet.Identity.RavenDB.Store
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
    public class UserStore<TUser, TRole, TKey> :
                                   GenericStore<TKey>,
                                   IUserStore<TUser, TKey>,
                                   IQueryableUserStore<TUser, TKey>,
                                   IUserClaimStore<TUser, TKey>,
                                   IUserEmailStore<TUser, TKey>,
                                   IUserLockoutStore<TUser, TKey>,
                                   IUserLoginStore<TUser, TKey>,
                                   IUserPasswordStore<TUser, TKey>,
                                   IUserPhoneNumberStore<TUser, TKey>,
                                   IUserRoleStore<TUser, TKey>,
                                   IUserSecurityStampStore<TUser, TKey>,
                                   IUserTwoFactorStore<TUser, TKey>

        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IConvertible, IComparable, IEquatable<TKey>
    {
        #region CTOR
        
        protected UserStore(Func<IAsyncDocumentSession> getSession)
            : base(getSession)
        {
            IndexInstaller().Wait();
        }

        protected UserStore(IAsyncDocumentSession session)
            : base(session)
        {
            IndexInstaller().Wait();
        }

        private async Task IndexInstaller()
        {
            var store = Session.Advanced.DocumentStore;

            await new IdentityRole_GetByName<TRole, TKey>().ExecuteAsync(store.AsyncDatabaseCommands, store.Conventions);
            await new IdentityUser_GetByEmail<TUser, TKey>().ExecuteAsync(store.AsyncDatabaseCommands, store.Conventions);
            await new IdentityUser_GetByUserName<TUser, TKey>().ExecuteAsync(store.AsyncDatabaseCommands, store.Conventions);
        }

        #endregion

        #region IUserStore

        public Task CreateAsync(TUser user)
        {
            base.CheckArgumentForNull(user, "user");

            this.Session.StoreAsync(user);

            return base.SaveChangesAsync();
        }

        public Task DeleteAsync(TUser user)
        {
            base.CheckArgumentForNull(user, "user");

            this.Session.Delete(user);

            return base.SaveChangesAsync();
        }

        public Task<TUser> FindByIdAsync(TKey userId)
        {
            base.CheckArgumentForNull(userId, "userId");

            TUser user = null;
            if (CheckInt())
            {
                var id = Convert.ToInt32(userId);
                user = this.Session.LoadAsync<TUser>(id).Result;
            }
            else if (CheckString())
            {
                user = this.Session.LoadAsync<TUser>(userId as string).Result;
            }
            else
            {
                ThrowTypeAccessException(typeof(TKey));
            }

            return Task.FromResult(user);
        }

        public Task<TUser> FindByNameAsync(string userName)
        {
            base.CheckArgumentForNull(userName, "userName");

            var user = Session.Query<TUser, IdentityUser_GetByUserName<TUser, TKey>>()
                                .SingleOrDefaultAsync(x => x.UserName == userName).Result;

            return Task.FromResult(user);
        }

        public Task UpdateAsync(TUser user)
        {
            base.CheckArgumentForNull(user, "user");

            return base.SaveChangesAsync();
        }

        #endregion

        #region IQueryableUserStore

        public IQueryable<TUser> Users
        {
            get
            {
                base.ThrowIfDisposed();
                return Session.Query<TUser>().AsQueryable();
            }
        }

        #endregion

        #region IUserClaimStore

        public Task AddClaimAsync(TUser user, Claim claim)
        {
            base.CheckArgumentForNull(user, "user");
            base.CheckArgumentForNull(claim, "claim");

            if (!user.Claims.Any(x => x.ClaimType == claim.Type && x.ClaimValue == claim.Value))
            {
                user.Claims.Add(new IdentityUserClaim(claim));
            }

            return base.SaveChangesAsync();
        }

        public Task<IList<Claim>> GetClaimsAsync(TUser user)
        {
            base.CheckArgumentForNull(user, "user");

            var result = user.Claims.Select(c => new Claim(c.ClaimType, c.ClaimValue)).ToIList();

            return Task.FromResult(result);
        }

        public Task RemoveClaimAsync(TUser user, Claim claim)
        {
            base.CheckArgumentForNull(user, "user");
            base.CheckArgumentForNull(claim, "claim");

            user.Claims.RemoveAll(x => x.ClaimType == claim.Type && x.ClaimValue == claim.Value);

            return base.SaveChangesAsync();
        }

        #endregion

        #region IUserEmailStore

        public Task<TUser> FindByEmailAsync(string email)
        {
            base.CheckArgumentForNull(email, "email");

            return Session.Query<TUser, IdentityUser_GetByEmail<TUser, TKey>>().SingleOrDefaultAsync(x => x.Email == email);
        }

        public Task<string> GetEmailAsync(TUser user)
        {
            base.CheckArgumentForNull(user, "user");

            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(TUser user)
        {
            base.CheckArgumentForNull(user, "user");

            return Task.FromResult(user.IsEmailConfirmed);
        }

        public Task SetEmailAsync(TUser user, string email)
        {
            base.CheckArgumentForNull(user, "user");
            base.CheckArgumentForOnlyNull(email, "email");

            user.Email = email;

            return base.SaveChangesAsync();
        }

        public Task SetEmailConfirmedAsync(TUser user, bool confirmed)
        {
            base.CheckArgumentForNull(user, "user");
            base.CheckArgumentForOnlyNull(confirmed, "confirmed");

            user.IsEmailConfirmed = confirmed;

            return base.SaveChangesAsync();
        }

        #endregion

        #region IUserLockoutStore


        public Task<int> GetAccessFailedCountAsync(TUser user)
        {
            base.CheckArgumentForNull(user, "user");

            return Task.FromResult(user.AccessFailedCount);
        }

        public Task<bool> GetLockoutEnabledAsync(TUser user)
        {
            base.CheckArgumentForNull(user, "user");

            return Task.FromResult(user.LockoutEnabled);
        }

        public Task<DateTimeOffset> GetLockoutEndDateAsync(TUser user)
        {
            base.CheckArgumentForNull(user, "user");

            return Task.FromResult(user.LockoutEndDate);
        }

        public Task<int> IncrementAccessFailedCountAsync(TUser user)
        {
            base.CheckArgumentForNull(user, "user");

            user.AccessFailedCount++;

            base.SaveChangesAsync().Wait();

            return Task.FromResult(user.AccessFailedCount);
        }

        public Task ResetAccessFailedCountAsync(TUser user)
        {
            base.CheckArgumentForNull(user, "user");

            user.AccessFailedCount = 0;

            return base.SaveChangesAsync();
        }

        public Task SetLockoutEnabledAsync(TUser user, bool enabled)
        {
            base.CheckArgumentForNull(user, "user");
            base.CheckArgumentForOnlyNull(enabled, "enabled");

            user.LockoutEnabled = enabled;

            return base.SaveChangesAsync();
        }

        public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset lockoutEnd)
        {
            base.CheckArgumentForNull(user, "user");
            base.CheckArgumentForNull(lockoutEnd, "lockoutEnd");

            user.LockoutEndDate = lockoutEnd;

            return base.SaveChangesAsync();
        }

        #endregion

        #region IUserLoginStore

        public Task AddLoginAsync(TUser user, UserLoginInfo login)
        {
            base.CheckArgumentForNull(user, "user");
            base.CheckArgumentForNull(login, "login");

            if (!user.Logins.Any(x => x.UserLoginInfo.LoginProvider == login.LoginProvider
                                                    && x.UserLoginInfo.ProviderKey == login.ProviderKey))
            {
                var identityUser = IdentityUserLogin<TKey>.CreateIdentityUserLogin();
                identityUser.Id = Helper.GetLoginId(login, this.GetIdentityPartsSeparator());
                identityUser.UserId = user.Id;
                identityUser.UserLoginInfo = login;

                user.Logins.Add(identityUser);

                this.Session.StoreAsync(identityUser);
            }

            return base.SaveChangesAsync();
        }

        public Task<TUser> FindAsync(UserLoginInfo login)
        {
            base.CheckArgumentForNull(login, "login");

            string loginId = Helper.GetLoginId(login, this.GetIdentityPartsSeparator());

            var loginDoc = Session.Include<IdentityUserLogin<TKey>>(x => x.UserId).LoadAsync(loginId).Result;

            TUser user = null;

            if (loginDoc != null)
            {
                if (CheckInt())
                {
                    user = this.Session.LoadAsync<TUser>(Convert.ToInt32(loginDoc.UserId)).Result;
                }
                if (CheckString())
                {
                    user = this.Session.LoadAsync<TUser>(loginDoc.UserId as string).Result;
                }
            }

            return Task.FromResult(user);
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user)
        {
            base.CheckArgumentForNull(user, "user");

            var result = user.Logins.Select(x => x.UserLoginInfo).ToIList();

            return Task.FromResult(result);
        }

        public Task RemoveLoginAsync(TUser user, UserLoginInfo login)
        {
            base.CheckArgumentForNull(user, "user");
            base.CheckArgumentForNull(login, "login");

            string loginId = Helper.GetLoginId(login, this.GetIdentityPartsSeparator());
            var loginDoc = this.Session.LoadAsync<IdentityUserLogin<TKey>>(loginId);
            if (loginDoc != null)
                this.Session.Delete(loginDoc);

            user.Logins.RemoveAll(x => x.UserLoginInfo.LoginProvider == login.LoginProvider
                                                     && x.UserLoginInfo.ProviderKey == login.ProviderKey);

            return base.SaveChangesAsync();
        }

        #endregion

        #region IUserPasswordStore

        public Task<string> GetPasswordHashAsync(TUser user)
        {
            base.CheckArgumentForNull(user, "user");

            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(TUser user)
        {
            base.CheckArgumentForNull(user, "user");

            return Task.FromResult(user.PasswordHash != null);
        }

        public Task SetPasswordHashAsync(TUser user, string passwordHash)
        {
            base.CheckArgumentForNull(user, "user");
            base.CheckArgumentForOnlyNull(passwordHash, "passwordHash");

            user.PasswordHash = passwordHash;

            return base.SaveChangesAsync();
        }

        #endregion

        #region IUserPhoneNumberStore

        public Task<string> GetPhoneNumberAsync(TUser user)
        {
            base.CheckArgumentForNull(user, "user");

            return Task.FromResult(user.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(TUser user)
        {
            base.CheckArgumentForNull(user, "user");

            return Task.FromResult(user.IsPhoneNumberConfirmed);
        }

        public Task SetPhoneNumberAsync(TUser user, string phoneNumber)
        {
            base.CheckArgumentForNull(user, "user");
            base.CheckArgumentForOnlyNull(phoneNumber, "phoneNumber");

            user.PhoneNumber = phoneNumber;

            return base.SaveChangesAsync();
        }

        public Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed)
        {
            base.CheckArgumentForNull(user, "user");
            base.CheckArgumentForOnlyNull(confirmed, "confirmed");

            user.IsPhoneNumberConfirmed = confirmed;

            return base.SaveChangesAsync();
        }

        #endregion

        #region IUserRoleStore

        public Task AddToRoleAsync(TUser user, string roleName)
        {
            base.CheckArgumentForNull(user, "user");
            base.CheckArgumentForNull(roleName, "roleName");

            var roleId = Session.Query<IdentityRole<TKey>, IdentityRole_GetByName<TRole, TKey>>()
                                    .SingleOrDefaultAsync(x => x.Name == roleName).Result.Id;

            if (!user.Roles.Contains(roleId))
                user.Roles.Add(roleId);

            return base.SaveChangesAsync();
        }

        public Task<IList<string>> GetRolesAsync(TUser user)
        {
            base.CheckArgumentForNull(user, "user");

            List<string> ids = new List<string>();
            if (CheckInt())
            {
                ids.AddRange(user.Roles.Select(role => base.CreateId(Convert.ToInt32(role), typeof(IdentityRole<int>))));
            }
            if (CheckString())
            {
                ids.AddRange(user.Roles.Select(role => role.ToString(CultureInfo.InvariantCulture)));
            }

            var result = Session.LoadAsync<IdentityRole<TKey>>(ids);
            if (result == null)
                return Task.FromResult(new List<string>().ToIList());

            return Task.FromResult(result.Result.Select(x => x.Name).ToIList());
        }

        public Task<bool> IsInRoleAsync(TUser user, string roleName)
        {
            base.CheckArgumentForNull(user, "user");
            base.CheckArgumentForNull(roleName, "roleName");

            var roleId = Session.Query<IdentityRole<TKey>>().SingleOrDefaultAsync(x => x.Name == roleName).Result.Id;

            return Task.FromResult(user.Roles.Contains<TKey>(roleId));
        }

        public Task RemoveFromRoleAsync(TUser user, string roleName)
        {
            base.CheckArgumentForNull(user, "user");
            base.CheckArgumentForNull(roleName, "roleName");

            var roleId = Session.Query<IdentityRole<TKey>>().SingleOrDefaultAsync(x => x.Name == roleName).Result.Id;

            user.Roles.RemoveAll(r => Equals(r, roleId));

            return base.SaveChangesAsync();
        }

        #endregion

        #region IUserSecurityStampStore

        public Task<string> GetSecurityStampAsync(TUser user)
        {
            base.CheckArgumentForNull(user, "user");

            return Task.FromResult(user.SecurityStamp);
        }

        public Task SetSecurityStampAsync(TUser user, string stamp)
        {
            base.CheckArgumentForNull(user, "user");
            base.CheckArgumentForNull(stamp, "stamp");

            user.SecurityStamp = stamp;

            return base.SaveChangesAsync();
        }

        #endregion

        #region IUserTwoFactorStore

        public Task<bool> GetTwoFactorEnabledAsync(TUser user)
        {
            base.CheckArgumentForNull(user, "user");

            return Task.FromResult(user.TwoFactorAuthEnabled);
        }

        public Task SetTwoFactorEnabledAsync(TUser user, bool enabled)
        {
            base.CheckArgumentForNull(user, "user");
            base.CheckArgumentForNull(enabled, "enabled");

            user.TwoFactorAuthEnabled = enabled;

            return base.SaveChangesAsync();
        }

        #endregion
    }
}
