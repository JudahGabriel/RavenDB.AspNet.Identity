using System;
using Blun.AspNet.Identity.RavenDB.Store;
using Microsoft.AspNet.Identity;
using Raven.Client;
using RavenDB.AspNet.Identity;

namespace Blun.AspNet.Identity.RavenDB
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
    public sealed class UserStore<TUser, TRole> : UserStore<TUser, TRole, string>,
                                            IUserStore<TUser>,
                                            IQueryableUserStore<TUser>,
                                            IUserClaimStore<TUser>,
                                            IUserEmailStore<TUser>,
                                            IUserLockoutStore<TUser, string>,
                                            IUserLoginStore<TUser>,
                                            IUserPasswordStore<TUser>,
                                            IUserPhoneNumberStore<TUser>,
                                            IUserRoleStore<TUser>,
                                            IUserSecurityStampStore<TUser>,
                                            IUserTwoFactorStore<TUser, string>
        where TUser : IdentityUser<string>, IUser
        where TRole : IdentityRole<string>, IRole
    {
        public UserStore(Func<IAsyncDocumentSession> getSession)
            : base(getSession)
        {
        }

        public UserStore(IAsyncDocumentSession session)
            : base(session)
        {
        }
    }
}
