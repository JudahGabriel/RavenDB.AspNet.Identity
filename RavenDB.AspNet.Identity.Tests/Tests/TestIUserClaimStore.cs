using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blun.AspNet.Identity.RavenDB.Entity;
using Blun.AspNet.Identity.RavenDB.Tests.Infrastructure;
using Blun.AspNet.Identity.RavenDB.Tests.Models;
using Microsoft.AspNet.Identity;
using Raven.Client;
using Xunit;

namespace Blun.AspNet.Identity.RavenDB.Tests.Tests
{
    public class TestIUserClaimStore : BaseTest
    {
        public TestIUserClaimStore()
        {
            _cleanUpRavenDBAktion = this.ClaenUpRavenDb;
        }

        private void ClaenUpRavenDb()
        {
            DeleteUser();
        }

        private void CreatUsers()
        {
            var demo = new SimpleUser()
            {
                Claims = new List<IdentityUserClaim>()
                {
                    new IdentityUserClaim()
                    {
                        ClaimType = "Email",
                        ClaimValue = "test@mail.com"
                    }
                },
                UserName = "ClaimTest"
            };

            _session.StoreAsync(new SimpleUser() { UserName = "Admin" });
            
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

        #region IUserClaimStore

        public void IUserClaimStore_GetClaimsAsync_Count_4()
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
                userResult = mgr.GetClaimsAsync().Result;
            }

            // Assert
            Assert.Null(userResult);
        }

        public void IUserClaimStore_AddClaimAsync_True()
        {
            
        }

        public void IUserClaimStore_RemoveClaimAsync_True()
        {
            
        }

        #endregion
    }
}
