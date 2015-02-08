using System;
using Blun.AspNet.Identity.RavenDB.Store;
using Raven.Client;
using RavenDB.AspNet.Identity;

namespace Blun.AspNet.Identity.RavenDB
{
    public sealed class UserStoreIntKey<TUser, TRole> : UserStore<TUser, TRole, int>
        where TUser : IdentityUser<int>
        where TRole : IdentityRole<int>
    {
        public UserStoreIntKey(Func<IAsyncDocumentSession> getSession)
            : base(getSession)
        {
        }

        public UserStoreIntKey(IAsyncDocumentSession session)
            : base(session)
        {
        }
    }
}