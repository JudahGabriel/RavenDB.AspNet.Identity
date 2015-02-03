using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Raven.Client;
using RavenDB.AspNet.Identity;

namespace Blun.AspNet.Identity.RavenDB.Store
{

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TRole"></typeparam>
    /// <typeparam name="TKey">only <see cref="string"/> or <see cref="int"/></typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
    public class RoleStore<TRole, TKey> :
                                GenericStore<TKey>,
                                IRoleStore<TRole, TKey>,
                                //IRoleClaimStore<TRole, TKey>,  vNext
                                IQueryableRoleStore<TRole, TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IConvertible, IComparable, IEquatable<TKey>
    {
        #region CTOR

        public RoleStore(Func<IAsyncDocumentSession> getSession)
            : base(getSession)
        {
        }

        public RoleStore(IAsyncDocumentSession session)
            : base(session)
        {
        }

        #endregion

        #region IRoleStore

        public Task CreateAsync(TRole role)
        {
            base.CheckArgumentForNull(role, "role");
            base.CheckArgumentForNull(role.Name, "role.Name");

            var checkRoleName = FindByNameAsync(role.Name).Result;
            if (checkRoleName == null)
            {
                 Session.StoreAsync(role).Wait();
            }

            return base.SaveChangesAsync();
        }

        public Task UpdateAsync(TRole role)
        {
            base.CheckArgumentForNull(role, "role");

            return base.SaveChangesAsync();
        }

        public Task DeleteAsync(TRole role)
        {
            base.CheckArgumentForNull(role, "role");

            Session.Delete(role);

            return base.SaveChangesAsync();
        }

        public Task<TRole> FindByIdAsync(TKey roleId)
        {
            base.CheckArgumentForNull(roleId, "roleId");

            TRole role = null;
            if (CheckInt())
            {
                var id = Convert.ToInt32(roleId);
                role = this.Session.LoadAsync<TRole>(id).Result;
            }
            else if (CheckString())
            {
                role = this.Session.LoadAsync<TRole>(roleId as string).Result;
            }
            else
            {
                ThrowTypeAccessException(typeof(TKey));
            }

            return Task.FromResult(role);
        }

        public Task<TRole> FindByNameAsync(string roleName)
        {
            base.CheckArgumentForNull(roleName, "roleName");

            TRole role = null;

            role = Session.Query<TRole>().SingleOrDefaultAsync(x => x.Name == roleName).Result;

            return Task.FromResult(role);
        }

        #endregion

        #region IQueryableRoleStore

        public IQueryable<TRole> Roles
        {
            get
            {
                base.ThrowIfDisposed();
                return Session.Query<TRole>().AsQueryable();
            }
        }

        #endregion
    }
}
