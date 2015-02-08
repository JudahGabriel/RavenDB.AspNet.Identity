using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blun.AspNet.Identity.RavenDB.Tests.Infrastructure;
using Blun.AspNet.Identity.RavenDB.Tests.Models;
using Microsoft.AspNet.Identity;
using NUnit.Framework;
using Raven.Client;

// ReSharper disable once CheckNamespace
namespace Blun.AspNet.Identity.RavenDB.Tests
{
    [TestFixture]
    public class UserStoreTest : BaseTest<SimpleUser>
    {
        
        public UserStoreTest()
            : base()
        {
            _cleanUpRavenDBAktion = this.ClaenUpRavenDb;
        }

        public async Task ClaenUpRavenDb()
        {
            await DeleteUsers();
        }

        private static async Task CreatUsers(IAsyncDocumentSession session, string prefix)
        {
            await session.StoreAsync(new SimpleUser() { UserName = prefix + "_" + "Admin" });
            await session.StoreAsync(new SimpleUser() { UserName = prefix + "_" + "Benutzer" });
            await session.StoreAsync(new SimpleUser() { UserName = prefix + "_" + "Reader" });
            await session.StoreAsync(new SimpleUser() { UserName = prefix + "_" + "Writer" });

            await session.SaveChangesAsync();

            await Task.Delay(_delay);
        }

        private async Task DeleteUsers(string prefix = "")
        {
            IList<SimpleUser> ids = new List<SimpleUser>();
            if (string.IsNullOrWhiteSpace(prefix))
            {
                ids = await GetSession().Query<SimpleUser>().ToListAsync();
            }
            else
            {
                ids = await GetSession().Query<SimpleUser>().Where(x => x.UserName.StartsWith(prefix) || x.UserName == prefix).ToListAsync();
            }

            foreach (var id in ids)
            {
                GetSession().Delete<SimpleUser>(id);
            }
            await GetSession().SaveChangesAsync();
        }

        #region IUserStore

        [Test]
        public async Task IUserStore_FindByNameAsync_Name_SimpleUser_False()
        {
            // Arrange
            const string name = "IUserStore_FindByNameAsync_Name_SimpleUser_False";
            var user = await StoreAsync(GetSession(), new SimpleUser()
            {
                UserName = name
            });
            var id = user.Id;
            SimpleUser userResult = null;

            // Act
            using (var mgr = new UserManager<SimpleUser>(new UserStore<SimpleUser, SimpleRole>(GetSession()) { AutoSaveChanges = true }))
            {
                userResult = await mgr.FindByNameAsync(name + "2");
            }

            // Assert
            Assert.Null(userResult);

            // TearDown
            await base.Delete(GetSession(), user);
        }


        [Test]
        public async Task IUserStore_FindByNameAsync_Name_SimpleUser_True()
        {
            // Arrange
            const string name = "IUserStore_FindByNameAsync_Name_SimpleUser_True";
            var user = await StoreAsync(GetSession(), new SimpleUser()
            {
                UserName = name
            });
            var id = user.Id;
            SimpleUser userResult = null;

            // Act
            using (var mgr = new UserManager<SimpleUser>(new UserStore<SimpleUser, SimpleRole>(GetSession()) { AutoSaveChanges = true }))
            {
                userResult = await mgr.FindByNameAsync(name);
            }

            // Assert
            Assert.NotNull(userResult);
            Assert.AreEqual(user.Id, userResult.Id);
            Assert.AreEqual(user.UserName, userResult.UserName);

            // TearDown
            await base.Delete(GetSession(), user);
        }


        [Test]
        public async Task IUserStore_FindByIdAsync_ID_SimpleUser_False()
        {
            // Arrange
            const string name = "IUserStore_FindByIdAsync_ID_SimpleUser_False";
            var user = await StoreAsync(GetSession(), new SimpleUser()
            {
                UserName = name
            });
            var id = user.Id;
            SimpleUser userResult = null;

            // Act
            using (var mgr = new UserManager<SimpleUser>(new UserStore<SimpleUser, SimpleRole>(GetSession()) { AutoSaveChanges = true }))
            {
                userResult = await mgr.FindByIdAsync(id + "321311312313213414214143214124214");
            }

            // Assert
            Assert.Null(userResult);

            // TearDown
            await base.Delete(GetSession(), user);
        }

        [Test]
        public async Task IUserStore_FindByIdAsync_ID_SimpleUser_True()
        {
            // Arrange
            const string name = "IUserStore_FindByIdAsync_ID_SimpleUser_True";
            var user = await StoreAsync(GetSession(), new SimpleUser()
            {
                UserName = name
            });
            var id = user.Id;
            SimpleUser userResult = null;

            // Act
            using (var mgr = new UserManager<SimpleUser>(new UserStore<SimpleUser, SimpleRole>(GetSession()) { AutoSaveChanges = true }))
            {
                userResult = await mgr.FindByIdAsync(id);
            }

            // Assert
            Assert.NotNull(userResult);
            Assert.AreEqual(user.Id, userResult.Id);
            Assert.AreEqual(user.UserName, userResult.UserName);

            // TearDown
            await base.Delete(GetSession(), user);
        }

        [Test]
        public async Task IUserStore_DeleteAsync_SimpleUser_ID_True()
        {
            // Arrange
            const string name = "IUserStore_DeleteAsync_SimpleUser_ID_True";
            var user = await StoreAsync(GetSession(), new SimpleUser()
            {
                UserName = name
            });
            var id = user.Id;

            // Act
            using (var mgr = new UserManager<SimpleUser>(new UserStore<SimpleUser, SimpleRole>(GetSession()) { AutoSaveChanges = true }))
            {
                await mgr.DeleteAsync(user);
            }

            // Assert
            var userResult = await GetSession().LoadAsync<SimpleUser>(id);
            Assert.Null(userResult);

            // TearDown
        }

        [Test]
        public async Task IUserStore_DeleteAsync_SimpleUser_ID_False()
        {
            // Arrange
            const string name = "IUserStore_DeleteAsync_SimpleUser_ID_False";
            var user = await StoreAsync(GetSession(), new SimpleUser()
            {
                UserName = name
            });
            var id = user.Id;
            Exception e = null;

            // Act
            using (var mgr = new UserManager<SimpleUser>(new UserStore<SimpleUser, SimpleRole>(GetSession()) { AutoSaveChanges = true }))
            {
                var act = new SimpleUser();
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
            var userResult = await GetSession().LoadAsync<SimpleUser>(id);
            Assert.NotNull(userResult);
            Assert.NotNull(e);
            Assert.IsInstanceOf<InvalidOperationException>(e);

            // TearDown
            await base.Delete(GetSession(), user);
        }

        [Test]
        public async Task IUserStore_UpdateAsync_SimpleUser_ID_ID_NAME_NAME()
        {
            // Arrange
            const string name = "IUserStore_UpdateAsync_SimpleUser_ID_ID_NAME_NAME";
            var user = await StoreAsync(GetSession(), new SimpleUser()
            {
                UserName = name + "1"
            });

            // Act
            using (var mgr = new UserManager<SimpleUser>(new UserStore<SimpleUser, SimpleRole>(GetSession()) { AutoSaveChanges = true }))
            {
                user.UserName = name;
                await mgr.UpdateAsync(user);
            }

            // Assert
            var userResult = await GetSession().LoadAsync<SimpleUser>(user.Id);
            Assert.AreEqual(user.Id, userResult.Id);
            Assert.AreEqual(user.UserName, name);

            // TearDown
            await base.Delete(GetSession(), user);
        }

        [Test]
        public async Task IUserStore_CreateAsync_SimpleUser_ID_True()
        {
            // Arrange
            var user = new SimpleUser()
            {
                UserName = "IUserStore_CreateAsync_SimpleUser_ID_True"
            };

            // Act
            using (var mgr = new UserManager<SimpleUser>(new UserStore<SimpleUser, SimpleRole>(GetSession()) { AutoSaveChanges = true }))
            {
                await mgr.CreateAsync(user);
            }

            // Assert
            Assert.True(!string.IsNullOrWhiteSpace(user.Id));

            // TearDown
            await base.Delete(GetSession(), user);
        }


        #endregion

        #region IQueryableUserStore

        [Test]
        public async Task IQueryableUserStore_ToList_Exception()
        {
            // Arrange
            const string prefix = "IQueryableUserStore_ToList_Exception";
            await CreatUsers(GetSession(), prefix);
            IList<SimpleUser> users = new List<SimpleUser>();
            Exception e = null;
            
            // Act
            using (var mgr = new UserManager<SimpleUser>(new UserStore<SimpleUser, SimpleRole>(GetSession()) { AutoSaveChanges = true }))
            {
                try
                {
                    users = mgr.Users.Where(x => x.UserName.StartsWith(prefix)).ToList();
                }
                catch (Exception ex)
                {
                    e = ex;
                }
            }

            // Assert
            Assert.True(!users.Any());
            Assert.NotNull(e);
            Assert.IsInstanceOf<NotSupportedException>(e);

            // TearDown
            await DeleteUsers(prefix);
        }

        [Test]
        public async Task IQueryableUserStore_ToListAsync_Count_4_True()
        {
            // Arrange
            const string prefix = "IQueryableUserStore_ToListAsync_Count_4_True";
            await CreatUsers(GetSession(), prefix);
            IList<SimpleUser> users = new List<SimpleUser>();

            // Act
            using (var mgr = new UserManager<SimpleUser>(new UserStore<SimpleUser, SimpleRole>(GetSession()) { AutoSaveChanges = true }))
            {
                users = await mgr.Users.Where(x => x.UserName.StartsWith(prefix)).ToListAsync();
            }

            // Assert
            Assert.True(users.Count() == 4);

            // TearDown
            await DeleteUsers(prefix);
        }

        #endregion

    }
}
