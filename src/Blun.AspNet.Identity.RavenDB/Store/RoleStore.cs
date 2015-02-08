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

        public async Task CreateAsync(TRole role)
        {
            base.CheckArgumentForNull(role, "role");
            base.CheckArgumentForNull(role.Name, "role.Name");

            var checkRoleName = await FindByNameAsync(role.Name);
            if (checkRoleName == null)
            {
                await base.Session.StoreAsync(role);
            }

            await base.SaveChangesAsync();
        }

        public async Task UpdateAsync(TRole role)
        {
            base.CheckArgumentForNull(role, "role");

            await base.SaveChangesAsync();
        }

        public async Task DeleteAsync(TRole role)
        {
            base.CheckArgumentForNull(role, "role");

            Session.Delete(role);

            await base.SaveChangesAsync();
        }

        public async Task<TRole> FindByIdAsync(TKey roleId)
        {
            base.CheckArgumentForNull(roleId, "roleId");

            TRole role = null;
            if (CheckInt())
            {
                var id = Convert.ToInt32(roleId);
                return await base.Session.LoadAsync<TRole>(id);
            }
            else if (CheckString())
            {
                return await base.Session.LoadAsync<TRole>(roleId as string);
            }
            else
            {
                ThrowTypeAccessException(typeof(TKey));
            }

            return await Task.FromResult(role);
        }

        public async Task<TRole> FindByNameAsync(string roleName)
        {
            base.CheckArgumentForNull(roleName, "roleName");

            return await Session.Query<TRole>().FirstOrDefaultAsync(x => x.Name == roleName);
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
