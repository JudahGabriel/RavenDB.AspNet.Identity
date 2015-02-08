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
using NUnit;
using NUnit.Framework;

// ReSharper disable once CheckNamespace
namespace Blun.AspNet.Identity.RavenDB.Tests
{
    [TestFixture]
    public class TestIUserClaimStore : BaseTest<SimpleUser>
    {
        public TestIUserClaimStore()
        {
            _cleanUpRavenDBAktion = this.ClaenUpRavenDb;
        }

        public async Task ClaenUpRavenDb()
        {
            await DeleteClaimStore();
        }

        private async Task CreatClaimStore()
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

            await GetSession().StoreAsync(new SimpleUser() { UserName = "Admin" });
            
            await GetSession().SaveChangesAsync();
            
        }

        private async Task DeleteClaimStore()
        {
            var ids = await GetSession().Query<SimpleUser>().ToListAsync();
            foreach (var id in ids)
            {
                GetSession().Delete<SimpleUser>(id);
            }
            await GetSession().SaveChangesAsync();
        }

        #region IUserClaimStore

        [Test]
        public async Task  IUserClaimStore_GetClaimsAsync_Count_4()
        {
            // Arrange
            const string name = "IUserClaimStore_GetClaimsAsync_Count_4";
            var user = await StoreAsync(GetSession(), new SimpleUser()
            {
                UserName = name
            });
            var id = user.Id;
            SimpleUser userResult = null;

            // Act
            using (var mgr = new UserManager<SimpleUser>(new UserStore<SimpleUser, SimpleRole>(GetSession()) { AutoSaveChanges = true }))
            {
                //userResult = mgr.GetClaimsAsync().Result;
            }

            // Assert
            Assert.IsNull(userResult);

            // TearDown
            await base.Delete(GetSession(), user);
        }

        public async Task  IUserClaimStore_AddClaimAsync_True()
        {
            
        }

        public async Task  IUserClaimStore_RemoveClaimAsync_True()
        {
            
        }

        #endregion
    }
}
