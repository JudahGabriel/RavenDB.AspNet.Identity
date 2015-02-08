using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Embedded;
using Raven.Client.Indexes;
using RavenDB.AspNet.Identity;

namespace Blun.AspNet.Identity.RavenDB.Tests.Infrastructure
{
    public abstract class BaseTest<TObject> : IDisposable where TObject : class
    {
        protected Func<IDocumentStore> _GetDocumentStore;
        protected const int _delay = 101;
        protected readonly IDocumentStore _docStore;
        private readonly Dictionary<string, IAsyncDocumentSession> _sessions;
        protected Func<Task> _cleanUpRavenDBAktion;

        protected BaseTest()
        {
            //change store for mem oder server
            _GetDocumentStore = ServerDocumentStore;

            //IDocumentStore
            _docStore = NewDocStore();
            _sessions = new Dictionary<string, IAsyncDocumentSession>();
        }


        protected async Task<TObject> StoreAsync(IAsyncDocumentSession session, TObject entity)
        {
            await session.StoreAsync(entity);
            await session.SaveChangesAsync();
            await Task.Delay(_delay);
            return entity;
        }

        protected async Task Delete(IAsyncDocumentSession session, TObject entity)
        {
            session.Delete(entity);
            await session.SaveChangesAsync();
        }

        #region IDocumentStore

        protected IAsyncDocumentSession GetSession([CallerMemberName]string sourceMemberName = "")
        {
            if (string.IsNullOrWhiteSpace(sourceMemberName))
            {
                throw new ArgumentNullException("key");
            }

            if (!_sessions.ContainsKey(sourceMemberName))
            {
                _sessions[sourceMemberName] = _docStore.OpenAsyncSession();
            }
            return _sessions[sourceMemberName];
        }

        protected IDocumentStore NewDocStore()
        {
            var store = _GetDocumentStore.Invoke();

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
                },
                UseEmbeddedHttpServer = true
            };


            return embeddedStore;
        }

        #endregion

        #region IDisposable

        public virtual void Dispose()
        {
            if (_cleanUpRavenDBAktion != null)
                _cleanUpRavenDBAktion.Invoke().Wait();

            foreach (var asyncDocumentSession in _sessions)
            {
                asyncDocumentSession.Value.Dispose();
            }

            _docStore.Dispose();
        }

        #endregion

    }
}
