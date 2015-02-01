using System;
using Raven.Client;
using RavenDB.AspNet.Identity.Store;

namespace RavenDB.AspNet.Identity
{
    public sealed class RoleStore<TRole> :
                                   RoleStore<TRole, string>
       where TRole : IdentityRole
    {
        public RoleStore(Func<IAsyncDocumentSession> getSession)
            : base(getSession)
        {
        }

        public RoleStore(IAsyncDocumentSession session)
            : base(session)
        {
        }
    }
}
