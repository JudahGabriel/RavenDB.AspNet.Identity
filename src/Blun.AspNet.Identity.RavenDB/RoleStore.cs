using System;
using Blun.AspNet.Identity.RavenDB.Store;
using Microsoft.AspNet.Identity;
using Raven.Client;
using RavenDB.AspNet.Identity;

namespace Blun.AspNet.Identity.RavenDB
{
    public sealed class RoleStore<TRole> :
                                   RoleStore<TRole, string>
       where TRole : IdentityRole, IRole
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
