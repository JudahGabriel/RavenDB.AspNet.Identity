using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Embedded;
using Raven.Client.Indexes;
using RavenDB.AspNet.Identity;
using RavenDB.AspNet.Identity.Store;

namespace RavenDB.AspNet.Identity.Tests
{
    public abstract class BaseTest
    {
        protected IDocumentStore NewServerDocumentStore()
        {
            var store = new DocumentStore()
            {
                Url = @"http://localhost:9090",
                DefaultDatabase = "test_identity"
            }.Initialize();

            return store;
        }

        protected EmbeddableDocumentStore NewDocStore()
        {
            var embeddedStore = new EmbeddableDocumentStore
            {
                Configuration =
                {
                    RunInMemory = true,
                    RunInUnreliableYetFastModeThatIsNotSuitableForProduction = true
                }
            };

            embeddedStore.Initialize();

            new RavenDocumentsByEntityName().Execute(embeddedStore);

            return embeddedStore;
        }

        protected UserStore<TUser, TRole> NewUserStore<TUser, TRole>(IDocumentStore docStore)
            where TUser : IdentityUser
            where TRole : IdentityRole
        {
            return new UserStore<TUser, TRole>(docStore.OpenAsyncSession);
        }

        protected UserManager<TUser> NewUserManager<TUser, TRole>(IDocumentStore docStore)
            where TUser : IdentityUser
            where TRole : IdentityRole
        {
            return new UserManager<TUser>(this.NewUserStore<TUser, TRole>(docStore));
        }
    }
}
