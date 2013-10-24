using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using RavenDB.AspNet.Identity;
using Xunit;
using Xunit.Extensions;
using Util = RavenDB.AspNet.Identity.Util;

namespace RavenDB.AspNet.Identity.Tests
{
	public class LoginStore : BaseTest
	{
		[Fact]
		public void Can_create_user()
		{
			string username = "DavidBoike";
			string password = Guid.NewGuid().ToString("n");

			var user = new SimpleAppUser { UserName = username };

			using (var docStore = NewDocStore())
			{
				using (var session = docStore.OpenSession())
				{
					using (var mgr = new UserManager<SimpleAppUser>(new UserStore<SimpleAppUser>(session)))
					{
						IdentityResult result = mgr.Create(user, password);

						Assert.True(result.Succeeded);
						Assert.NotNull(user.Id);

						var res1 = mgr.AddLogin(user.Id, new UserLoginInfo("Google", "http://www.google.com/fake/user/identifier"));
						var res2 = mgr.AddLogin(user.Id, new UserLoginInfo("Yahoo", "http://www.yahoo.com/fake/user/identifier"));

						Assert.True(res1.Succeeded);
						Assert.True(res2.Succeeded);
					}
					session.SaveChanges();
				}

				using (var session = docStore.OpenSession())
				{
					var loaded = session.Load<SimpleAppUser>(user.Id);
					Assert.NotNull(loaded);
					Assert.NotSame(loaded, user);
					Assert.Equal(loaded.Id, user.Id);
					Assert.Equal(loaded.UserName, user.UserName);
					Assert.NotNull(loaded.PasswordHash);

					Assert.Equal(loaded.Logins.Count, 2);
					Assert.True(loaded.Logins.Any(x => x.LoginProvider == "Google" && x.ProviderKey == "http://www.google.com/fake/user/identifier"));
					Assert.True(loaded.Logins.Any(x => x.LoginProvider == "Yahoo" && x.ProviderKey == "http://www.yahoo.com/fake/user/identifier"));

					var loadedLogins = session.Advanced.LoadStartingWith<IdentityUserLogin>("IdentityUserLogins/");
					Assert.Equal(loadedLogins.Length, 2);

					foreach (var login in loaded.Logins)
					{
						var loginDoc = session.Load<IdentityUserLogin>(Util.GetLoginId(login));
						Assert.Equal(login.LoginProvider, loginDoc.Provider);
						Assert.Equal(login.ProviderKey, loginDoc.ProviderKey);
						Assert.Equal(user.Id, loginDoc.UserId);
					}
				}
			}
		}
	}
}
