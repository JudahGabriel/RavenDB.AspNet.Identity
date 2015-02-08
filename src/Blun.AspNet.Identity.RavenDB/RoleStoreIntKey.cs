using System;
using Blun.AspNet.Identity.RavenDB.Store;
using Raven.Client;
using RavenDB.AspNet.Identity;

namespace Blun.AspNet.Identity.RavenDB
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