using System;
using Raven.Client;
using RavenDB.AspNet.Identity.Store;

namespace RavenDB.AspNet.Identity
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