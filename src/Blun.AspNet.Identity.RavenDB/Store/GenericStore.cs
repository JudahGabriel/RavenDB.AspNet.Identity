using System;
using System.Threading.Tasks;
using Blun.AspNet.Identity.RavenDB.Common;
using Raven.Client;
using Raven.Client.Document.Async;

namespace Blun.AspNet.Identity.RavenDB.Store
{
    /// <summary>
    /// /the base for the <see cref="UserStore"/> and <see cref="Rolestore"/>
    /// </summary>
    /// <typeparam name="TKey">only <see cref="string"/> or <see cref="int"/></typeparam>
    public abstract class GenericStore<TKey> : 
                                        GenericBase<TKey>
        where TKey : IConvertible, IComparable, IEquatable<TKey>
    {
        #region Fields

        private readonly Func<IAsyncDocumentSession> _getSessionFunc;
        private IAsyncDocumentSession _session;
        private Func<string> _identityPartsSeparatorActor;

        #endregion

        #region CTOR

        /// <summary>
        /// Better nev
        /// </summary>
        protected GenericStore()
            : base()
        {
            //IDisposable
            HandleDisposable = Disposeable;
            
            AutoSaveChanges = true;
            //_session.Advanced.DocumentStore.Conventions.RegisterIdConvention<TUser>((dbname, commands, user) => user.KeyPrefix + this.GetIdentityPartsSeparator() + user.Id);
        }


        /// <summary>
        /// Use it for Lazy use for <see cref="AsyncDocumentSession"/>
        /// </summary>
        /// <param name="getSession">delegate for <see cref="AsyncDocumentSession"/></param>
        protected GenericStore(Func<IAsyncDocumentSession> getSession)
            : this()
        {
            this.CheckArgumentForNull(getSession, "getSession");

            this._getSessionFunc = getSession;
        }

        /// <summary>
        /// Use it if the <see cref="AsyncDocumentSession"/> is now used
        /// </summary>
        /// <param name="session"></param>
        protected GenericStore(IAsyncDocumentSession session)
            : this()
        {
            this.CheckArgumentForNull(session, "session");
            this._session = session;
        }

        #endregion

        #region IDisposable

        protected void Disposeable()
        {
            _session.Dispose();
        }

        #endregion

        #region Methods/Functions

        protected string CreateId(int id, Type type)
        {
            return Session.Advanced.DocumentStore.Conventions.FindFullDocumentKeyFromNonStringIdentifier(id, type, false);
        }

        /// <summary>
        /// If it is 'true' it would automated save the RavenDb, otherwise the developer had to manuel to do
        /// </summary>
        public bool AutoSaveChanges { get; set; }


        /// <summary>
        /// Saves changes if <see cref="AutoSaveChanges"/> is 'true'
        /// </summary>
        /// <returns><see cref="Task"/></returns>
        protected async Task SaveChangesAsync()
        {
            base.ThrowIfDisposed();
            if ( AutoSaveChanges)
            {
                await Session.SaveChangesAsync();
            }
            else
            {
                await Task.FromResult(0);
            }
        }

        /// <summary>
        /// Return the RavenDB IdentityPartsSeparator from client session
        /// </summary>
        protected Func<string> GetIdentityPartsSeparator
        {
            get
            {
                Func<string> standard = () => @"/";
                if (_identityPartsSeparatorActor == null)
                {
                    return standard;
                }
                return _identityPartsSeparatorActor;
            }
        }

        /// <summary>
        /// The RavenDb session
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
        protected IAsyncDocumentSession Session
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_session.Advanced.DocumentStore.Conventions.IdentityPartsSeparator))
                {
                    this._identityPartsSeparatorActor = () => _session.Advanced.DocumentStore.Conventions.IdentityPartsSeparator;
                }
                if (_session == null && this._getSessionFunc != null)
                {
                    this._session = this._getSessionFunc();
                }
                else if (_session == null)
                {
                    throw new NullReferenceException("_session");
                }
                return _session;
            }
        }

        #endregion

    }
}