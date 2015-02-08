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

        public async Task CreateAsync(TUser user)
        {
            base.CheckArgumentForNull(user, "user");

            await this.Session.StoreAsync(user);

            await base.SaveChangesAsync();
        }

        public async Task DeleteAsync(TUser user)
        {
            base.CheckArgumentForNull(user, "user");

            this.Session.Delete(user);

            await base.SaveChangesAsync();
        }

        public async Task<TUser> FindByIdAsync(TKey userId)
        {
            base.CheckArgumentForNull(userId, "userId");

            TUser user = default(TUser);
            if (CheckInt())
            {
                var id = Convert.ToInt32(userId);
                return await this.Session.LoadAsync<TUser>(id);
            }
            else if (CheckString())
            {
                return await this.Session.LoadAsync<TUser>(userId as string);
            }
            else
            {
                ThrowTypeAccessException(typeof(TKey));
            }

            return await Task.FromResult(user);
        }

        public async Task<TUser> FindByNameAsync(string userName)
        {
            base.CheckArgumentForNull(userName, "userName");

            return await base.Session.Query<TUser, IdentityUser_GetByUserName<TUser, TKey>>()
                                .SingleOrDefaultAsync(x => x.UserName == userName);
        }

        public async Task UpdateAsync(TUser user)
        {
            base.CheckArgumentForNull(user, "user");

            await base.SaveChangesAsync();
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

        public async Task AddClaimAsync(TUser user, Claim claim)
        {
            base.CheckArgumentForNull(user, "user");
            base.CheckArgumentForNull(claim, "claim");

            if (!user.Claims.Any(x => x.ClaimType == claim.Type && x.ClaimValue == claim.Value))
            {
                user.Claims.Add(new IdentityUserClaim(claim));
            }

            await base.SaveChangesAsync();
        }

        public async Task<IList<Claim>> GetClaimsAsync(TUser user)
        {
            base.CheckArgumentForNull(user, "user");

            var result = user.Claims.Select(c => new Claim(c.ClaimType, c.ClaimValue)).ToIList();

            return await Task.FromResult(result);
        }

        public async Task RemoveClaimAsync(TUser user, Claim claim)
        {
            base.CheckArgumentForNull(user, "user");
            base.CheckArgumentForNull(claim, "claim");

            user.Claims.RemoveAll(x => x.ClaimType == claim.Type && x.ClaimValue == claim.Value);

            await base.SaveChangesAsync();
        }

        #endregion

        #region IUserEmailStore

        public async Task<TUser> FindByEmailAsync(string email)
        {
            base.CheckArgumentForNull(email, "email");

            return await Session.Query<TUser, IdentityUser_GetByEmail<TUser, TKey>>().SingleOrDefaultAsync(x => x.Email == email);
        }

        public async Task<string> GetEmailAsync(TUser user)
        {
            base.CheckArgumentForNull(user, "user");

            return await Task.FromResult(user.Email);
        }

        public async Task<bool> GetEmailConfirmedAsync(TUser user)
        {
            base.CheckArgumentForNull(user, "user");

            return await Task.FromResult(user.IsEmailConfirmed);
        }

        public async Task SetEmailAsync(TUser user, string email)
        {
            base.CheckArgumentForNull(user, "user");
            base.CheckArgumentForOnlyNull(email, "email");

            user.Email = email;

            await base.SaveChangesAsync();
        }

        public async Task SetEmailConfirmedAsync(TUser user, bool confirmed)
        {
            base.CheckArgumentForNull(user, "user");
            base.CheckArgumentForOnlyNull(confirmed, "confirmed");

            user.IsEmailConfirmed = confirmed;

            await base.SaveChangesAsync();
        }

        #endregion

        #region IUserLockoutStore

        public async Task<int> GetAccessFailedCountAsync(TUser user)
        {
            base.CheckArgumentForNull(user, "user");

            return await Task.FromResult(user.AccessFailedCount);
        }

        public async Task<bool> GetLockoutEnabledAsync(TUser user)
        {
            base.CheckArgumentForNull(user, "user");

            return await Task.FromResult(user.LockoutEnabled);
        }

        public async Task<DateTimeOffset> GetLockoutEndDateAsync(TUser user)
        {
            base.CheckArgumentForNull(user, "user");

            return await Task.FromResult(user.LockoutEndDate);
        }

        public async Task<int> IncrementAccessFailedCountAsync(TUser user)
        {
            base.CheckArgumentForNull(user, "user");

            user.AccessFailedCount++;

            await base.SaveChangesAsync();

            return await Task.FromResult(user.AccessFailedCount);
        }

        public async Task ResetAccessFailedCountAsync(TUser user)
        {
            base.CheckArgumentForNull(user, "user");

            user.AccessFailedCount = 0;

            await base.SaveChangesAsync();
        }

        public async Task SetLockoutEnabledAsync(TUser user, bool enabled)
        {
            base.CheckArgumentForNull(user, "user");
            base.CheckArgumentForOnlyNull(enabled, "enabled");

            user.LockoutEnabled = enabled;

            await base.SaveChangesAsync();
        }

        public async Task SetLockoutEndDateAsync(TUser user, DateTimeOffset lockoutEnd)
        {
            base.CheckArgumentForNull(user, "user");
            base.CheckArgumentForNull(lockoutEnd, "lockoutEnd");

            user.LockoutEndDate = lockoutEnd;

            await base.SaveChangesAsync();
        }

        #endregion

        #region IUserLoginStore

        public async Task AddLoginAsync(TUser user, UserLoginInfo login)
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

                await this.Session.StoreAsync(identityUser);
            }

            await base.SaveChangesAsync();
        }

        public async Task<TUser> FindAsync(UserLoginInfo login)
        {
            base.CheckArgumentForNull(login, "login");

            string loginId = Helper.GetLoginId(login, this.GetIdentityPartsSeparator());

            var loginDoc = await Session.Include<IdentityUserLogin<TKey>>(x => x.UserId).LoadAsync(loginId);

            TUser user = null;

            if (loginDoc != null)
            {
                if (CheckInt())
                {
                    user = await this.Session.LoadAsync<TUser>(Convert.ToInt32(loginDoc.UserId));
                }
                if (CheckString())
                {
                    user = await this.Session.LoadAsync<TUser>(loginDoc.UserId as string);
                }
            }

            return await Task.FromResult(user);
        }

        public async Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user)
        {
            base.CheckArgumentForNull(user, "user");

            var result = user.Logins.Select(x => x.UserLoginInfo).ToIList();

            return await Task.FromResult(result);
        }

        public async Task RemoveLoginAsync(TUser user, UserLoginInfo login)
        {
            base.CheckArgumentForNull(user, "user");
            base.CheckArgumentForNull(login, "login");

            string loginId = Helper.GetLoginId(login, this.GetIdentityPartsSeparator());
            var loginDoc = await this.Session.LoadAsync<IdentityUserLogin<TKey>>(loginId);
            if (loginDoc != null)
                this.Session.Delete(loginDoc);

            user.Logins.RemoveAll(x => x.UserLoginInfo.LoginProvider == login.LoginProvider
                                                     && x.UserLoginInfo.ProviderKey == login.ProviderKey);

            await base.SaveChangesAsync();
        }

        #endregion

        #region IUserPasswordStore

        public async Task<string> GetPasswordHashAsync(TUser user)
        {
            base.CheckArgumentForNull(user, "user");

            return await Task.FromResult(user.PasswordHash);
        }

        public async Task<bool> HasPasswordAsync(TUser user)
        {
            base.CheckArgumentForNull(user, "user");

            return await Task.FromResult(user.PasswordHash != null);
        }

        public async Task SetPasswordHashAsync(TUser user, string passwordHash)
        {
            base.CheckArgumentForNull(user, "user");
            base.CheckArgumentForOnlyNull(passwordHash, "passwordHash");

            user.PasswordHash = passwordHash;

            await base.SaveChangesAsync();
        }

        #endregion

        #region IUserPhoneNumberStore

        public async Task<string> GetPhoneNumberAsync(TUser user)
        {
            base.CheckArgumentForNull(user, "user");

            return await Task.FromResult(user.PhoneNumber);
        }

        public async Task<bool> GetPhoneNumberConfirmedAsync(TUser user)
        {
            base.CheckArgumentForNull(user, "user");

            return await Task.FromResult(user.IsPhoneNumberConfirmed);
        }

        public async Task SetPhoneNumberAsync(TUser user, string phoneNumber)
        {
            base.CheckArgumentForNull(user, "user");
            base.CheckArgumentForOnlyNull(phoneNumber, "phoneNumber");

            user.PhoneNumber = phoneNumber;

            await base.SaveChangesAsync();
        }

        public async Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed)
        {
            base.CheckArgumentForNull(user, "user");
            base.CheckArgumentForOnlyNull(confirmed, "confirmed");

            user.IsPhoneNumberConfirmed = confirmed;

            await base.SaveChangesAsync();
        }

        #endregion

        #region IUserRoleStore

        public async Task AddToRoleAsync(TUser user, string roleName)
        {
            base.CheckArgumentForNull(user, "user");
            base.CheckArgumentForNull(roleName, "roleName");

            var roleId = (await Session.Query<IdentityRole<TKey>, IdentityRole_GetByName<TRole, TKey>>()
                                    .SingleOrDefaultAsync(x => x.Name == roleName)).Id;

            if (!user.Roles.Contains(roleId))
                user.Roles.Add(roleId);

            await base.SaveChangesAsync();
        }

        public async Task<IList<string>> GetRolesAsync(TUser user)
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

            var result = await Session.LoadAsync<IdentityRole<TKey>>(ids);
            if (result == null)
                return await Task.FromResult(new List<string>().ToIList());

            return await Task.FromResult(result.Select(x => x.Name).ToIList());
        }

        public async Task<bool> IsInRoleAsync(TUser user, string roleName)
        {
            base.CheckArgumentForNull(user, "user");
            base.CheckArgumentForNull(roleName, "roleName");

            var roleId = (await Session.Query<IdentityRole<TKey>>().SingleOrDefaultAsync(x => x.Name == roleName)).Id;

            return await Task.FromResult(user.Roles.Contains<TKey>(roleId));
        }

        public async Task RemoveFromRoleAsync(TUser user, string roleName)
        {
            base.CheckArgumentForNull(user, "user");
            base.CheckArgumentForNull(roleName, "roleName");

            var roleId = (await Session.Query<IdentityRole<TKey>>().SingleOrDefaultAsync(x => x.Name == roleName)).Id;

            user.Roles.RemoveAll(r => Equals(r, roleId));

            await base.SaveChangesAsync();
        }

        #endregion

        #region IUserSecurityStampStore

        public async Task<string> GetSecurityStampAsync(TUser user)
        {
            base.CheckArgumentForNull(user, "user");

            return await Task.FromResult(user.SecurityStamp);
        }

        public async Task SetSecurityStampAsync(TUser user, string stamp)
        {
            base.CheckArgumentForNull(user, "user");
            base.CheckArgumentForNull(stamp, "stamp");

            user.SecurityStamp = stamp;

            await base.SaveChangesAsync();
        }

        #endregion

        #region IUserTwoFactorStore

        public async Task<bool> GetTwoFactorEnabledAsync(TUser user)
        {
            base.CheckArgumentForNull(user, "user");

            return await Task.FromResult(user.TwoFactorAuthEnabled);
        }

        public async Task SetTwoFactorEnabledAsync(TUser user, bool enabled)
        {
            base.CheckArgumentForNull(user, "user");
            base.CheckArgumentForNull(enabled, "enabled");

            user.TwoFactorAuthEnabled = enabled;

            await base.SaveChangesAsync();
        }

        #endregion
        
    }
}
