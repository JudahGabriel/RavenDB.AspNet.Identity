using System;
using System.Linq;
using Blun.AspNet.Identity.RavenDB.Common;
using Blun.AspNet.Identity.RavenDB.Tests.Infrastructure;
using Blun.AspNet.Identity.RavenDB.Tests.Models;
using Microsoft.AspNet.Identity;
using NUnit.Framework;
using RavenDB.AspNet.Identity;
using NUnit;

// ReSharper disable once CheckNamespace
namespace Blun.AspNet.Identity.RavenDB.Tests
{
    [TestFixture]
    [Ignore]
    public class LoginStoreTest : BaseTest<SimpleUser>
    {
        [Test]
        public void Can_create_user_and_log_in()
        {
            const string username = "DavidBoike";
            const string userId = "user_id_1";
            string password = Guid.NewGuid().ToString("n");
            const string googleLogin = "http://www.google.com/fake/user/identifier";
            const string yahooLogin = "http://www.yahoo.com/fake/user/identifier";

            var user = new SimpleUser { Id = userId, UserName = username };


            using (
                var mgr =
                    new UserManager<SimpleUser>(new UserStore<SimpleUser, SimpleRole>(GetSession())
                    {
                        AutoSaveChanges = true
                    }))
            {
                IdentityResult result = mgr.Create(user, password);

                Assert.True(result.Succeeded);
                Assert.NotNull(user.Id);

                var res1 = mgr.AddLoginAsync(user.Id, new UserLoginInfo("Google", googleLogin)).Result;
                var res2 = mgr.AddLoginAsync(user.Id, new UserLoginInfo("Yahoo", yahooLogin)).Result;

                Assert.True(res1.Succeeded);
                Assert.True(res2.Succeeded);
            }
            GetSession().SaveChangesAsync().Wait();
            
            var loaded = GetSession().LoadAsync<SimpleUser>(user.Id).Result;
            Assert.NotNull(loaded);
            //Assert.NotSame(loaded, user);
            Assert.AreEqual(loaded.Id, user.Id);
            Assert.AreEqual(loaded.UserName, user.UserName);
            Assert.NotNull(loaded.PasswordHash);

            Assert.AreEqual(loaded.Logins.Count, 2);
            Assert.True(
                loaded.Logins.Any(
                    x => x.UserLoginInfo.LoginProvider == "Google" && x.UserLoginInfo.ProviderKey == googleLogin));
            Assert.True(
                loaded.Logins.Any(
                    x => x.UserLoginInfo.LoginProvider == "Yahoo" && x.UserLoginInfo.ProviderKey == yahooLogin));

            var loadedLogins = GetSession().Advanced.LoadStartingWithAsync<IdentityUserLogin>("IdentityUserLogins/").Result;
            Assert.AreEqual(loadedLogins.Count(), 2);

            foreach (var login in loaded.Logins)
            {
                var loginDoc = GetSession().LoadAsync<IdentityUserLogin>(Helper.GetLoginId(login.UserLoginInfo)).Result;
                Assert.AreEqual(login.UserLoginInfo.LoginProvider, loginDoc.UserLoginInfo.LoginProvider);
                Assert.AreEqual(login.UserLoginInfo.ProviderKey, loginDoc.UserLoginInfo.ProviderKey);
                Assert.AreEqual(user.Id, loginDoc.UserId);
            }
            GetSession().SaveChangesAsync().Wait();


            using (var mgr = new UserManager<SimpleUser>(new UserStore<SimpleUser, SimpleRole>(GetSession())))
            {
                var userByName = mgr.Find(username, password);
                var userByGoogle = mgr.Find(new UserLoginInfo("Google", googleLogin));
                var userByYahoo = mgr.Find(new UserLoginInfo("Yahoo", yahooLogin));

                Assert.NotNull(userByName);
                Assert.NotNull(userByGoogle);
                Assert.NotNull(userByYahoo);

                Assert.AreEqual(userByName.Id, userId);
                Assert.AreEqual(userByName.UserName, username);

                // The Session cache should return the very same objects
                Assert.AreSame(userByName, userByGoogle);
                Assert.AreSame(userByName, userByYahoo);
            }
            GetSession().SaveChangesAsync().Wait();
        }
    }
}
