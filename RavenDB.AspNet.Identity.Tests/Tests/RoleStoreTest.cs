using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Blun.AspNet.Identity.RavenDB.Tests.Infrastructure;
using Blun.AspNet.Identity.RavenDB.Tests.Models;
using Microsoft.AspNet.Identity;
using Raven.Abstractions.Exceptions;
using Raven.Client;
using Raven.Client.Extensions;
using RavenDB.AspNet.Identity;
using Xunit;

// ReSharper disable once CheckNamespace
namespace Blun.AspNet.Identity.RavenDB.Tests
{
    public class RoleStoreTest : BaseTest
    {
        public RoleStoreTest()
            : base()
        {
            _cleanUpRavenDBAktion = this.ClaenUpRavenDb;
        }

        private void ClaenUpRavenDb()
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
        public void IRoleStore_FindByIdAsync_Name_SimpleRole_False()
        {
            // Arrange
            const string name = "FindByIdAsync";
            var role = new SimpleRole()
            {
                Name = name
            };
            _session.StoreAsync(role).Wait();
            _session.SaveChangesAsync().Wait();
            var id = role.Id;
            SimpleRole roleResult = null;

            // Act
            using (var mgr = new RoleManager<SimpleRole>(new RoleStore<SimpleRole>(_session) { AutoSaveChanges = true }))
            {
                roleResult = mgr.FindByNameAsync(name + "2").Result;
            }

            // Assert
            Assert.Null(roleResult);
        }

        [Fact]
        public void IRoleStore_FindByIdAsync_Name_SimpleRole_True()
        {
            // Arrange
            const string name = "FindByIdAsync";
            var role = new SimpleRole()
            {
                Name = name
            };
            _session.StoreAsync(role).Wait();
            _session.SaveChangesAsync().Wait();
            var id = role.Id;
            SimpleRole roleResult = null;

            // Act
            using (var mgr = new RoleManager<SimpleRole>(new RoleStore<SimpleRole>(_session) { AutoSaveChanges = true }))
            {
                roleResult = mgr.FindByNameAsync(name).Result;
            }

            // Assert
            Assert.NotNull(roleResult);
            Assert.Equal(role.Id, roleResult.Id);
            Assert.Equal(role.Name, roleResult.Name);
        }

        [Fact]
        public void IRoleStore_FindByIdAsync_ID_SimpleRole_False()
        {
            // Arrange
            const string name = "FindByIdAsync";
            var role = new SimpleRole()
            {
                Name = name
            };
            _session.StoreAsync(role).Wait();
            _session.SaveChangesAsync().Wait();
            var id = role.Id;
            SimpleRole roleResult = null;

            // Act
            using (var mgr = new RoleManager<SimpleRole>(new RoleStore<SimpleRole>(_session) { AutoSaveChanges = true }))
            {
                roleResult = mgr.FindByIdAsync(id + "32947298472984").Result;
            }

            // Assert
            Assert.Null(roleResult);
        }

        [Fact]
        public void IRoleStore_FindByIdAsync_ID_SimpleRole_True()
        {
            // Arrange
            const string name = "FindByIdAsync";
            var role = new SimpleRole()
            {
                Name = name
            };
            _session.StoreAsync(role).Wait();
            _session.SaveChangesAsync().Wait();
            var id = role.Id;
            SimpleRole roleResult = null;

            // Act
            using (var mgr = new RoleManager<SimpleRole>(new RoleStore<SimpleRole>(_session) { AutoSaveChanges = true }))
            {
                roleResult = mgr.FindByIdAsync(id).Result;
            }

            // Assert
            Assert.NotNull(roleResult);
            Assert.Equal(role.Id, roleResult.Id);
            Assert.Equal(role.Name, roleResult.Name);
        }

        [Fact]
        public void IRoleStore_DeleteAsync_SimpleRole_ID_True()
        {
            // Arrange
            const string name = "DeleteAsync";
            var role = new SimpleRole()
            {
                Name = name
            };
            _session.StoreAsync(role).Wait();
            _session.SaveChangesAsync().Wait();
            var id = role.Id;

            // Act
            using (var mgr = new RoleManager<SimpleRole>(new RoleStore<SimpleRole>(_session) { AutoSaveChanges = true }))
            {
                mgr.DeleteAsync(role).Wait();
            }

            // Assert
            var roleResult = _session.LoadAsync<SimpleRole>(id).Result;
            Assert.Null(roleResult);
        }

        [Fact]
        public void IRoleStore_DeleteAsync_SimpleRole_ID_Fale()
        {
            // Arrange
            const string name = "DeleteAsync";
            var role = new SimpleRole()
            {
                Name = name
            };
            _session.StoreAsync(role).Wait();
            _session.SaveChangesAsync().Wait();
            var id = role.Id;
            Exception e = null;

            // Act
            using (var mgr = new RoleManager<SimpleRole>(new RoleStore<SimpleRole>(_session) { AutoSaveChanges = true }))
            {
                var act = new SimpleRole();
                try
                {
                    mgr.DeleteAsync(act).Wait();
                }
                catch (Exception ex)
                {
                    e = ex;
                }
            }

            // Assert
            var roleResult = _session.LoadAsync<SimpleRole>(id).Result;
            Assert.NotNull(roleResult);
            Assert.NotNull(e);
            Assert.IsType<InvalidOperationException>(e.InnerException);
            Assert.IsType<AggregateException>(e);
        }

        [Fact]
        public void IRoleStore_UpdateAsync_SimpleRole_ID_ID_NAME_NAME()
        {
            // Arrange
            const string name = "UpdateAsync";
            var role = new SimpleRole()
            {
                Name = name + "1"
            };
            _session.StoreAsync(role).Wait();
            _session.SaveChangesAsync().Wait();

            // Act
            using (var mgr = new RoleManager<SimpleRole>(new RoleStore<SimpleRole>(_session) { AutoSaveChanges = true }))
            {
                role.Name = name;
                mgr.UpdateAsync(role).Wait();
            }

            // Assert
            var roleResult = _session.LoadAsync<SimpleRole>(role.Id).Result;
            Assert.Equal(role.Id, roleResult.Id);
            Assert.Equal(role.Name, name);
        }

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
