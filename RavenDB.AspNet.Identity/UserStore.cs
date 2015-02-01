using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Raven.Client;
using RavenDB.AspNet.Identity.Store;

namespace RavenDB.AspNet.Identity
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
