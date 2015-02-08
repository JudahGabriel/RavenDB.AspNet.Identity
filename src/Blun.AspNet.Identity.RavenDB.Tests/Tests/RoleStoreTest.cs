using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Blun.AspNet.Identity.RavenDB.Tests.Infrastructure;
using Blun.AspNet.Identity.RavenDB.Tests.Models;
using Microsoft.AspNet.Identity;
using Raven.Abstractions.Exceptions;
using Raven.Client;
using Raven.Client.Extensions;
using RavenDB.AspNet.Identity;
using NUnit;
using NUnit.Framework;
using Raven.Client.Linq;
using Raven.Database.Storage.Voron.Impl;

// ReSharper disable once CheckNamespace
namespace Blun.AspNet.Identity.RavenDB.Tests
{
    [TestFixture]
    public class RoleStoreTest : BaseTest<SimpleRole>
    {
        
        public RoleStoreTest()
            : base()
        {
            _cleanUpRavenDBAktion = this.ClaenUpRavenDb;
        }

        private async Task ClaenUpRavenDb()
        {
            await DeleteRoles();
        }

        private static async Task CreatRoles(IAsyncDocumentSession session, string prefix)
        {
            await session.StoreAsync(new SimpleRole() { Name = prefix + "_" + "Admin" });
            await session.StoreAsync(new SimpleRole() { Name = prefix + "_" + "Benutzer" });
            await session.StoreAsync(new SimpleRole() { Name = prefix + "_" + "Reader" });
            await session.StoreAsync(new SimpleRole() { Name = prefix + "_" + "Writer" });

            await session.SaveChangesAsync();
            await Task.Delay(_delay);
        }

        private async Task DeleteRoles(string prefix = "")
        {
            IList<SimpleRole> ids = new List<SimpleRole>();
            if (string.IsNullOrWhiteSpace(prefix))
            {
                ids = await GetSession().Query<SimpleRole>().ToListAsync();
            }
            else
            {
                ids = await GetSession().Query<SimpleRole>().Where(x => x.Name.StartsWith(prefix) || x.Name == prefix).ToListAsync();
            }

            foreach (var id in ids)
            {
                GetSession().Delete<SimpleRole>(id);
            }
            await GetSession().SaveChangesAsync();
        }

        #region IRoleStore

        [Test]
        public async Task IRoleStore_FindByNameAsync_Name_SimpleRole_False()
        {
            // Arrange
            const string name = "IRoleStore_FindByNameAsync_Name_SimpleRole_False";
            var role = await StoreAsync(GetSession(), new SimpleRole()
            {
                Name = name
            });
            var id = role.Id;
            SimpleRole roleResult = null;
    
            // Act
            using (var mgr = new RoleManager<SimpleRole>(new RoleStore<SimpleRole>(GetSession()) { AutoSaveChanges = true }))
            {
                roleResult = await mgr.FindByNameAsync(name + "2");
            }

            // Assert
            Assert.Null(roleResult);

            // TearDown
            await base.Delete(GetSession(), role);
        }

        [Test]
        public async Task IRoleStore_FindByNameAsync_Name_SimpleRole_True()
        {
            // Arrange
            const string name = "IRoleStore_FindByNameAsync_Name_SimpleRole_True";
            var role = await StoreAsync(GetSession(), new SimpleRole()
            {
                Name = name
            });
            var id = role.Id;
            SimpleRole roleResult = null;

            // Act
            using (var mgr = new RoleManager<SimpleRole>(new RoleStore<SimpleRole>(GetSession()) { AutoSaveChanges = true }))
            {
                roleResult = await mgr.FindByNameAsync(name);
            }

            // Assert
            Assert.NotNull(roleResult);
            Assert.AreEqual(role.Id, roleResult.Id);
            Assert.AreEqual(role.Name, roleResult.Name);

            // TearDown
            await base.Delete(GetSession(), role);
        }

        [Test]
        public async Task IRoleStore_FindByIdAsync_ID_SimpleRole_False()
        {
            // Arrange
            const string name = "IRoleStore_FindByIdAsync_ID_SimpleRole_False";
            var role = await StoreAsync(GetSession(), new SimpleRole()
            {
                Name = name
            });
            var id = role.Id;
            SimpleRole roleResult = null;

            // Act
            using (var mgr = new RoleManager<SimpleRole>(new RoleStore<SimpleRole>(GetSession()) { AutoSaveChanges = true }))
            {
                roleResult = await mgr.FindByIdAsync(id + "32947298472984");
            }

            // Assert
            Assert.Null(roleResult);

            // TearDown
            await base.Delete(GetSession(), role);
        }

        [Test]
        public async Task IRoleStore_FindByIdAsync_ID_SimpleRole_True()
        {
            // Arrange
            const string name = "IRoleStore_FindByIdAsync_ID_SimpleRole_True";
            var role = await StoreAsync(GetSession(), new SimpleRole()
            {
                Name = name
            });
            var id = role.Id;
            SimpleRole roleResult = null;

            // Act
            using (var mgr = new RoleManager<SimpleRole>(new RoleStore<SimpleRole>(GetSession()) { AutoSaveChanges = true }))
            {
                roleResult = await mgr.FindByIdAsync(id);
            }

            // Assert
            Assert.NotNull(roleResult);
            Assert.AreEqual(role.Id, roleResult.Id);
            Assert.AreEqual(role.Name, roleResult.Name);

            // TearDown
            await base.Delete(GetSession(), role);
        }

        [Test]
        public async Task IRoleStore_DeleteAsync_SimpleRole_ID_True()
        {
            // Arrange
            const string name = "IRoleStore_DeleteAsync_SimpleRole_ID_True";
            var role = await StoreAsync(GetSession(), new SimpleRole()
            {
                Name = name
            });
            var id = role.Id;

            // Act
            using (var mgr = new RoleManager<SimpleRole>(new RoleStore<SimpleRole>(GetSession()) { AutoSaveChanges = true }))
            {
                await mgr.DeleteAsync(role);
            }

            // Assert
            var roleResult = await GetSession().LoadAsync<SimpleRole>(id);
            Assert.Null(roleResult);

            // TearDown
        }

        [Test]
        public async Task IRoleStore_DeleteAsync_SimpleRole_ID_Fale()
        {
            // Arrange
            const string name = "IRoleStore_DeleteAsync_SimpleRole_ID_Fale";
            var role = await StoreAsync(GetSession(), new SimpleRole()
            {
                Name = name
            });
            var id = role.Id;
            Exception e = null;

            // Act
            using (var mgr = new RoleManager<SimpleRole>(new RoleStore<SimpleRole>(GetSession()) { AutoSaveChanges = true }))
            {
                var act = new SimpleRole();
                try
                {
                    await mgr.DeleteAsync(act);
                }
                catch (Exception ex)
                {
                    e = ex;
                }
            }

            // Assert
            var roleResult = await GetSession().LoadAsync<SimpleRole>(id);
            Assert.NotNull(roleResult);
            Assert.NotNull(e);
            Assert.IsInstanceOf<InvalidOperationException>(e);

            // TearDown
            await base.Delete(GetSession(), role);
        }

        [Test]
        public async Task IRoleStore_UpdateAsync_SimpleRole_ID_ID_NAME_NAME()
        {
            // Arrange
            const string name = "IRoleStore_UpdateAsync_SimpleRole_ID_ID_NAME_NAME";
            var role = await StoreAsync(GetSession(), new SimpleRole()
            {
                Name = name + "1"
            });

            // Act
            using (var mgr = new RoleManager<SimpleRole>(new RoleStore<SimpleRole>(GetSession()) { AutoSaveChanges = true }))
            {
                role.Name = name;
                await mgr.UpdateAsync(role);
            }

            // Assert
            var roleResult = await GetSession().LoadAsync<SimpleRole>(role.Id);
            Assert.AreEqual(role.Id, roleResult.Id);
            Assert.AreEqual(role.Name, name);

            // TearDown
            await base.Delete(GetSession(), role);
        }

        [Test]
        public async Task IRoleStore_CreateAsync_SimpleRole_ID_True()
        {
            // Arrange
            const string name = "IRoleStore_CreateAsync_SimpleRole_ID_True";
            var role = new SimpleRole()
            {
                Name = name
            };


            // Act
            using (var mgr = new RoleManager<SimpleRole>(new RoleStore<SimpleRole>(GetSession()) { AutoSaveChanges = true }))
            {
                await mgr.CreateAsync(role);
            }

            // Assert
            Assert.True(!string.IsNullOrWhiteSpace(role.Id));

            // TearDown
            await base.Delete(GetSession(), role);
        }

        #endregion

        #region IQueryableRoleStore

        [Test]
        public async Task IQueryableRoleStore_ToList_Exception()
        {
            // Arrange
            const string prefix = "IQueryableRoleStore_ToList_Exception";
            await CreatRoles(GetSession(), prefix);
            IList<SimpleRole> roles = new List<SimpleRole>();
            Exception e = null;

            // Act
            using (var mgr = new RoleManager<SimpleRole>(new RoleStore<SimpleRole>(GetSession()) { AutoSaveChanges = true }))
            {
                try
                {
                    roles = mgr.Roles.Where(x => x.Name.StartsWith(prefix)).ToList();
                }
                catch (Exception ex)
                {
                    e = ex;
                }
            }

            // Assert
            Assert.True(!roles.Any());
            Assert.NotNull(e);
            Assert.IsInstanceOf<NotSupportedException>(e);

            // TearDown
            await DeleteRoles();
        }

        [Test]
        public async Task IQueryableRoleStore_ToListAsync_Count_4_True()
        {
            // Arrange
            const string prefix = "IQueryableRoleStore_ToListAsync_Count_4_True";
            await CreatRoles(GetSession(), prefix);
            IList<SimpleRole> roles = new List<SimpleRole>();

            // Act
            using (var mgr = new RoleManager<SimpleRole>(new RoleStore<SimpleRole>(GetSession()) { AutoSaveChanges = true }))
            {
                roles = await mgr.Roles.Where(x => x.Name.StartsWith(prefix)).ToListAsync();
            }

            // Assert
            Assert.True(roles.Count() == 4);

            // TearDown
            await DeleteRoles();
        }

        #endregion
    }
}
