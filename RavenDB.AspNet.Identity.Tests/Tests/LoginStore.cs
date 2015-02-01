using System;
using System.Linq;
using Microsoft.AspNet.Identity;
using RavenDB.AspNet.Identity.Common;
using Xunit;
using Xunit.Extensions;

// ReSharper disable once CheckNamespace
namespace RavenDB.AspNet.Identity.Tests
{
    public class LoginStore : BaseTest
    {
        [Fact]
        public void Can_create_user_and_log_in()
        {
            const string username = "DavidBoike";
            const string userId = "user_id_1";
            string password = Guid.NewGuid().ToString("n");
            const string googleLogin = "http://www.google.com/fake/user/identifier";
            const string yahooLogin = "http://www.yahoo.com/fake/user/identifier";

            var user = new SimpleAppUser { Id = userId, UserName = username };

            using (var docStore = NewDocStore())
            {
                using (var session = docStore.OpenAsyncSession())
                {
                    using (var mgr = new UserManager<SimpleAppUser>(new UserStore<SimpleAppUser, IdentityRole>(session) { AutoSaveChanges = false }))
                    {
                        IdentityResult result = mgr.Create(user, password);

                        Assert.True(result.Succeeded);
                        Assert.NotNull(user.Id);

                        var res1 = mgr.AddLoginAsync(user.Id, new UserLoginInfo("Google", googleLogin)).Result;
                        var res2 = mgr.AddLoginAsync(user.Id, new UserLoginInfo("Yahoo", yahooLogin)).Result;

                        Assert.True(res1.Succeeded);
                        Assert.True(res2.Succeeded);
                    }
                    session.SaveChangesAsync().Wait();
                }

                using (var session = docStore.OpenAsyncSession())
                {
                    var loaded = session.LoadAsync<SimpleAppUser>(user.Id).Result;
                    Assert.NotNull(loaded);
                    Assert.NotSame(loaded, user);
                    Assert.Equal(loaded.Id, user.Id);
                    Assert.Equal(loaded.UserName, user.UserName);
                    Assert.NotNull(loaded.PasswordHash);

                    Assert.Equal(loaded.Logins.Count, 2);
                    Assert.True(loaded.Logins.Any(x => x.UserLoginInfo.LoginProvider == "Google" && x.UserLoginInfo.ProviderKey == googleLogin));
                    Assert.True(loaded.Logins.Any(x => x.UserLoginInfo.LoginProvider == "Yahoo" && x.UserLoginInfo.ProviderKey == yahooLogin));

                    var loadedLogins = session.Advanced.LoadStartingWithAsync<IdentityUserLogin>("IdentityUserLogins/").Result;
                    Assert.Equal(loadedLogins.Count(), 2);

                    foreach (var login in loaded.Logins)
                    {
                        var loginDoc = session.LoadAsync<IdentityUserLogin>(Helper.GetLoginId(login.UserLoginInfo)).Result;
                        Assert.Equal(login.UserLoginInfo.LoginProvider, loginDoc.UserLoginInfo.LoginProvider);
                        Assert.Equal(login.UserLoginInfo.ProviderKey, loginDoc.UserLoginInfo.ProviderKey);
                        Assert.Equal(user.Id, loginDoc.UserId);
                    }
                    session.SaveChangesAsync().Wait();
                }

                using (var session = docStore.OpenAsyncSession())
                {
                    using (var mgr = new UserManager<SimpleAppUser>(new UserStore<SimpleAppUser, IdentityRole>(session)))
                    {
                        var userByName = mgr.Find(username, password);
                        var userByGoogle = mgr.Find(new UserLoginInfo("Google", googleLogin));
                        var userByYahoo = mgr.Find(new UserLoginInfo("Yahoo", yahooLogin));

                        Assert.NotNull(userByName);
                        Assert.NotNull(userByGoogle);
                        Assert.NotNull(userByYahoo);

                        Assert.Equal(userByName.Id, userId);
                        Assert.Equal(userByName.UserName, username);

                        // The Session cache should return the very same objects
                        Assert.Same(userByName, userByGoogle);
                        Assert.Same(userByName, userByYahoo);
                    }
                    session.SaveChangesAsync().Wait();
                }
            }
        }
    }
}
