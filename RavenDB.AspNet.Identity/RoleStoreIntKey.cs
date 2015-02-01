using System;
using Raven.Client;

namespace RavenDB.AspNet.Identity.Store
{
    public sealed class RoleStoreIntKey<TRole> :
        RoleStore<TRole, int>
        where TRole : IdentityRole<int>
    {
        public RoleStoreIntKey(Func<IAsyncDocumentSession> getSession)
            : base(getSession)
        {
        }

        public RoleStoreIntKey(IAsyncDocumentSession session)
            : base(session)
        {
        }
    }
}