using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blun.AspNet.Identity.RavenDB.Store;
using Blun.AspNet.Identity.RavenDB.Tests.Infrastructure;
using Blun.AspNet.Identity.RavenDB.Tests.Models;
using Microsoft.AspNet.Identity;
using Raven.Client;
using Xunit;

// ReSharper disable once CheckNamespace
namespace Blun.AspNet.Identity.RavenDB.Tests
{
    public class UserStoreTest : BaseTest
    {
        public UserStoreTest()
            : base()
        {
            _cleanUpRavenDBAktion = this.ClaenUpRavenDb;
        }

        private void ClaenUpRavenDb()
        {
            DeleteUser();
        }

        private void CreatUsers()
        {
            _session.StoreAsync(new SimpleUser() { UserName = "Admin" });
            _session.StoreAsync(new SimpleUser() { UserName = "Benutzer" });
            _session.StoreAsync(new SimpleUser() { UserName = "Reader" });
            _session.StoreAsync(new SimpleUser() { UserName = "Writer" });
            Task.WaitAll();
            _session.SaveChangesAsync();
            Task.WaitAll();
        }

        private void DeleteUser()
        {
            var ids = _session.Query<SimpleUser>().ToListAsync().Result;
            foreach (var id in ids)
            {
                _session.Delete<SimpleUser>(id);
            }
            _session.SaveChangesAsync();
            Task.WaitAll();
        }

        #region IUserStore

        [Fact]
        public void IUserStore_FindByIdAsync_Name_SimpleUser_False()
        {
            // Arrange
            const string name = "FindByIdAsync";
            var user = new SimpleUser()
            {
                UserName = name
            };
            _session.StoreAsync(user).Wait();
            _session.SaveChangesAsync().Wait();
            var id = user.Id;
            SimpleUser userResult = null;

            // Act
            using (var mgr = new UserManager<SimpleUser>(new UserStore<SimpleUser, SimpleRole>(_session) { AutoSaveChanges = true }))
            {
                userResult = mgr.FindByNameAsync(name + "2").Result;
            }

            // Assert
            Assert.Null(userResult);
        }


        [Fact]
        public void IUserStore_FindByIdAsync_Name_SimpleUser_True()
        {
            // Arrange
            const string name = "FindByIdAsync";
            var user = new SimpleUser()
            {
                UserName = name
            };
            _session.StoreAsync(user).Wait();
            _session.SaveChangesAsync().Wait();
            var id = user.Id;
            SimpleUser userResult = null;

            // Act
            using (var mgr = new UserManager<SimpleUser>(new UserStore<SimpleUser, SimpleRole>(_session) { AutoSaveChanges = true }))
            {
                userResult = mgr.FindByNameAsync(name).Result;
            }

            // Assert
            Assert.NotNull(userResult);
            Assert.Equal(user.Id, userResult.Id);
            Assert.Equal(user.UserName, userResult.UserName);
        }


        [Fact]
        public void IUserStore_FindByIdAsync_ID_SimpleUser_False()
        {
            // Arrange
            const string name = "FindByIdAsync";
            var user = new SimpleUser()
            {
                UserName = name
            };
            _session.StoreAsync(user).Wait();
            _session.SaveChangesAsync().Wait();
            var id = user.Id;
            SimpleUser userResult = null;

            // Act
            using (var mgr = new UserManager<SimpleUser>(new UserStore<SimpleUser, SimpleRole>(_session) { AutoSaveChanges = true }))
            {
                userResult = mgr.FindByIdAsync(id).Result;
            }

            // Assert
            Assert.Null(userResult);
        }

        [Fact]
        public void IUserStore_FindByIdAsync_ID_SimpleUser_True()
        {
            // Arrange
            const string name = "FindByIdAsync";
            var user = new SimpleUser()
            {
                UserName = name
            };
            _session.StoreAsync(user).Wait();
            _session.SaveChangesAsync().Wait();
            var id = user.Id;
            SimpleUser userResult = null;

            // Act
            using (var mgr = new UserManager<SimpleUser>(new UserStore<SimpleUser, SimpleRole>(_session) { AutoSaveChanges = true }))
            {
                userResult = mgr.FindByIdAsync(id).Result;
            }

            // Assert
            Assert.NotNull(userResult);
            Assert.Equal(user.Id, userResult.Id);
            Assert.Equal(user.UserName, userResult.UserName);
        }

        [Fact]
        public void IUserStore_DeleteAsync_SimpleUser_ID_True()
        {
            // Arrange
            const string name = "DeleteAsync";
            var user = new SimpleUser()
            {
                UserName = name
            };
            _session.StoreAsync(user).Wait();
            _session.SaveChangesAsync().Wait();
            var id = user.Id;

            // Act
            using (var mgr = new UserManager<SimpleUser>(new UserStore<SimpleUser, SimpleRole>(_session) { AutoSaveChanges = true }))
            {
                mgr.DeleteAsync(user).Wait();
            }

            // Assert
            var userResult = _session.LoadAsync<SimpleUser>(id).Result;
            Assert.Null(userResult);
        }

        [Fact]
        public void IUserStore_DeleteAsync_SimpleUser_ID_Fale()
        {
            // Arrange
            const string name = "DeleteAsync";
            var user = new SimpleUser()
            {
                UserName = name
            };
            _session.StoreAsync(user).Wait();
            _session.SaveChangesAsync().Wait();
            var id = user.Id;
            Exception e = null;

            // Act
            using (var mgr = new UserManager<SimpleUser>(new UserStore<SimpleUser, SimpleRole>(_session) { AutoSaveChanges = true }))
            {
                var act = new SimpleUser();
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
            var userResult = _session.LoadAsync<SimpleUser>(id).Result;
            Assert.NotNull(userResult);
            Assert.NotNull(e);
            Assert.IsType<InvalidOperationException>(e);
        }

        [Fact]
        public void IUserStore_UpdateAsync_SimpleUser_ID_ID_NAME_NAME()
        {
            // Arrange
            const string name = "UpdateAsync";
            var user = new SimpleUser()
            {
                UserName = name + "1"
            };
            _session.StoreAsync(user).Wait();
            _session.SaveChangesAsync().Wait();

            // Act
            using (var mgr = new UserManager<SimpleUser>(new UserStore<SimpleUser, SimpleRole>(_session) { AutoSaveChanges = true }))
            {
                user.UserName = name;
                mgr.UpdateAsync(user).Wait();
            }

            // Assert
            var userResult = _session.LoadAsync<SimpleUser>(user.Id).Result;
            Assert.Equal(user.Id, userResult.Id);
            Assert.Equal(user.UserName, name);
        }

        [Fact]
        public void IUserStore_CreateAsync_SimpleUser_ID_True()
        {
            // Arrange
            var user = new SimpleUser()
            {
                UserName = "CreateAsync"
            };

            // Act
            using (var mgr = new UserManager<SimpleUser>(new UserStore<SimpleUser, SimpleRole>(_session) { AutoSaveChanges = false }))
            {
                mgr.CreateAsync(user).Wait();
                _session.SaveChangesAsync().Wait();
            }

            // Assert
            Assert.True(!string.IsNullOrWhiteSpace(user.Id));
        }


        #endregion
            
        #region IQueryableUserStore

        [Fact]
        public void IQueryableUserStore_ToList_Exception()
        {
            // Arrange
            CreatUsers();
            IList<SimpleUser> users = new List<SimpleUser>();
            Exception e = null;
            // Act
            using (var mgr = new UserManager<SimpleUser>(new UserStore<SimpleUser, SimpleRole>(_session) { AutoSaveChanges = false }))
            {
                try
                {
                    users = mgr.Users.ToList();
                }
                catch (Exception ex)
                {
                    e = ex;
                }
            }

            // Assert
            Assert.True(!users.Any());
            Assert.NotNull(e);
            Assert.IsType<NotSupportedException>(e);
        }

        [Fact]
        public void IQueryableUserStore_ToListAsync_Count_4_True()
        {
            // Arrange
            CreatUsers();
            IList<SimpleUser> users = new List<SimpleUser>();

            // Act
            using (var mgr = new UserManager<SimpleUser>(new UserStore<SimpleUser, SimpleRole>(_session) { AutoSaveChanges = false }))
            {
                users = mgr.Users.ToListAsync().Result;
            }

            // Assert
            Assert.True(users.Count() == 4);
        }

        #endregion

    }
}
