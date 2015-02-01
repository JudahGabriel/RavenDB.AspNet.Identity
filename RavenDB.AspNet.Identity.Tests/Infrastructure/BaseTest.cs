using System;
using Microsoft.AspNet.Identity;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Embedded;
using Raven.Client.Indexes;
using RavenDB.AspNet.Identity;

namespace Blun.AspNet.Identity.RavenDB.Tests.Infrastructure
{
    public abstract class BaseTest : IDisposable
    {
        protected Func<IDocumentStore> _GetDocumentStore;

        protected readonly IDocumentStore _docStore;
        protected readonly IAsyncDocumentSession _session;
        protected Action _cleanUpRavenDBAktion;
        
        protected BaseTest()
        {
            //change store for mem oder server
            _GetDocumentStore = ServerDocumentStore;

            //IDocumentStore
            _docStore = NewDocStore();
            _session = _docStore.OpenAsyncSession();
        }

        #region IDocumentStore

        protected IDocumentStore NewDocStore()
        {
            var store = _GetDocumentStore.Invoke() ;

            store.Initialize();

            new RavenDocumentsByEntityName().Execute(store);

            return store;
        }

        protected IDocumentStore ServerDocumentStore()
        {
            var store = new DocumentStore()
            {
                Url = @"http://localhost:9090",
                DefaultDatabase = "test_identity"
            };
            
            return store;
        }

        protected EmbeddableDocumentStore EmbeddableDocumentStore()
        {
            var embeddedStore = new EmbeddableDocumentStore
            {
                Configuration =
                {
                    RunInMemory = true,
                    RunInUnreliableYetFastModeThatIsNotSuitableForProduction = true
                }
            };

            return embeddedStore;
        }

        #endregion

        #region IDisposable

        public virtual void Dispose()
        {
            if (_cleanUpRavenDBAktion != null)
                _cleanUpRavenDBAktion.Invoke();

            _session.Dispose();
            _docStore.Dispose();
        }

        #endregion


        //protected RoleManager<TRole> NewRoleStore<TRole>(IDocumentStore docStore)
        //    where TRole : IdentityRole
        //{
        //    return new RoleStore<TRole>(docStore.OpenAsyncSession);
        //}

        //protected UserStore<TUser, TRole> NewUserStore<TUser, TRole>(IDocumentStore docStore)
        //    where TUser : IdentityUser
        //    where TRole : IdentityRole
        //{
        //    return new UserStore<TUser, TRole>(docStore.OpenAsyncSession);
        //}

        //protected UserManager<TUser> NewUserManager<TUser, TRole>(IDocumentStore docStore)
        //    where TUser : IdentityUser
        //    where TRole : IdentityRole
        //{
        //    return new UserManager<TUser>(this.NewUserStore<TUser, TRole>(docStore));
        //}
       
    }
}
