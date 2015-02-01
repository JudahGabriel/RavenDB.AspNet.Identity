using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blun.AspNet.Identity.RavenDB.Tests.Infrastructure;
using Blun.AspNet.Identity.RavenDB.Tests.Models;
using Microsoft.AspNet.Identity;
using Raven.Client;
using Raven.Client.Extensions;
using RavenDB.AspNet.Identity;
using Xunit;

// ReSharper disable once CheckNamespace
namespace Blun.AspNet.Identity.RavenDB.Tests
{
    public class RoleStore : BaseTest
    {

        public RoleStore()
            : base()
        {
            _cleanUpRavenDBAktion = this.ClaeUpRavenDb;
        }

        private void ClaeUpRavenDb()
        {
            DeleteRoles();
        }

        private void CreatRoles()
        {
            _session.StoreAsync(new SimpleRole() { Name = "Admin" });
            _session.StoreAsync(new SimpleRole() { Name = "Benutzer" });
            _session.StoreAsync(new SimpleRole() { Name = "Reader" });
            _session.StoreAsync(new SimpleRole() { Name = "Writer" });
            Task.WaitAll();
            _session.SaveChangesAsync();
            Task.WaitAll();
        }

        private void DeleteRoles()
        {
            var ids = _session.Query<SimpleRole>().ToListAsync().Result;
            foreach (var id in ids)
            {
                _session.Delete<SimpleRole>(id);
            }
            _session.SaveChangesAsync();
            Task.WaitAll();
        }

        #region IRoleStore

        [Fact]
        public void IRoleStore_CreateAsync_SimpleRole_ID_True()
        {
            // Arrange
            var role = new SimpleRole()
            {
                Name = "CreateAsync"
            };

            // Act
            using (var mgr = new RoleManager<SimpleRole>(new RoleStore<SimpleRole>(_session) { AutoSaveChanges = false }))
            {
                mgr.CreateAsync(role).Wait();
                _session.SaveChangesAsync().Wait();
            }

            // Assert
            Assert.True(!string.IsNullOrWhiteSpace(role.Id));
        }

        #endregion

        #region IQueryableRoleStore

        [Fact]
        public void IQueryableRoleStore_ToList_Exception()
        {
            // Arrange
            CreatRoles();
            IList<SimpleRole> roles = new List<SimpleRole>();
            Exception e = null;
            // Act
            using (var mgr = new RoleManager<SimpleRole>(new RoleStore<SimpleRole>(_session) { AutoSaveChanges = false }))
            {
                try
                {
                    roles = mgr.Roles.ToList();
                }
                catch (Exception ex)
                {
                    e = ex;
                }
            }

            // Assert
            Assert.True(!roles.Any());
            Assert.NotNull(e);
            Assert.IsType<NotSupportedException>(e);
        }

        [Fact]
        public void IQueryableRoleStore_ToListAsync_Count_4_True()
        {
            // Arrange
            CreatRoles();
            IList<SimpleRole> roles = new List<SimpleRole>();

            // Act
            using (var mgr = new RoleManager<SimpleRole>(new RoleStore<SimpleRole>(_session) { AutoSaveChanges = false }))
            {
                roles = mgr.Roles.ToListAsync().Result;
            }

            // Assert
            Assert.True(roles.Count() == 4);
        }

        #endregion
    }
}
