# RavenDB.AspNet.Identity #
An ASP.NET Identity provider for RavenDB

## Instructions ##
These instructions assume you know how to set up RavenDB within an MVC application.

1. Create a new ASP.NET MVC 5 project, choosing the Individual User Accounts authentication type.
2. Remove the Entity Framework packages and replace with RavenDB Identity:
 
    Uninstall-Package Microsoft.AspNet.Identity.EntityFramework
    Uninstall-Package EntityFramework
    Install-Package RavenDB.AspNet.Identity
    
3. In ~/Models/IdentityModels.cs:
    * Remove the namespace: Microsoft.AspNet.Identity.EntityFramework
    * Add the namespace: RavenDB.AspNet.Identity
    * Remove the entire ApplicationDbContext class. You don't need that!
4. In ~/Controllers/AccountController.cs
    * Remove the namespace: Microsoft.AspNet.Identity.EntityFramework
    * Replace the UserStore in the constructor, providing a lambda expression to show the UserStore how to get the current IDocumentSession.
   
    // This example assumes you have a RavenController base class with public RavenSession property.
    public AccountController()
    {
        this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(() => this.RavenSession));
    }