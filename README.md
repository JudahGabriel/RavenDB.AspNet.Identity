# ![RavenDB logo](https://github.com/JudahGabriel/RavenDB.Identity/blob/master/RavenDB.Identity/nuget-icon.png?raw=true) RavenDB.AspNet.Identity #
RavenDB identity provider for ASP.NET MVC 5+ and Web API 2+. (Looking for .NET Core identity provider for RavenDB? Check out our [sister project, RavenDB.Identity](https://github.com/JudahGabriel/RavenDB.Identity).)

We're on [NuGet as RavenDB.AspNet.Identity](https://www.nuget.org/packages/RavenDB.AspNet.Identity/).

## Instructions ##
These instructions assume you know how to set up RavenDB within an MVC application.

1. Create a new ASP.NET MVC 5 project, choosing the Individual User Accounts authentication type.
2. Remove the Entity Framework packages and replace with RavenDB Identity:
 
    Uninstall-Package Microsoft.AspNet.Identity.EntityFramework
    Uninstall-Package EntityFramework
    Install-Package RavenDB.AspNet.Identity
    
3. In ~/Models/IdentityModels.cs:
    * Remove the namespace: Microsoft.AspNet.Identity.EntityFramework
    * Add the namespace: Raven.AspNet.Identity
    * Remove the entire ApplicationDbContext class. You don't need that!
4. In ~/App_Start/IdentityConfig.cs
    * Update the ApplicationUserManager.Create method to get the Raven document session.
   
    public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context) 
	{
		// Update this line to pass in the Raven document session:
		var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<IAsyncDocumentSession>()));
		...
	}
5. In ~/App_Start/Startup.Auth.cs:
	* Remove the Entity framework context and add a RavenDB context:
	// Old: app.CreatePerOwinContext(ApplicationDbContext.Create);
	// New. The raven variable is your Raven DocumentStore singleton.
	app.CreatePerOwinContext(() => raven.OpenAsyncSession());
6. Add a RavenController base class. 
	* This will save changes on the document session if the controller action executed successfully.
	* You can [view the RavenController.cs sample](https://github.com/JudahGabriel/RavenDB.AspNet.Identity/blob/master/Sample/Controllers/RavenController.cs).
7. Make AccountController.cs inherit from RavenController
	public class AccountController : RavenController
	{
		...
	}
