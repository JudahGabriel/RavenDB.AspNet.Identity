# RavenDB.AspNet.Identity #
An ASP.NET Identity provider for RavenDB.
Thanks to 'ilmservice' wich create the first Version of RavenDB.AspNet.Identity.

## Alpha Version ##
Beware it is the first apha realese. I need more time for mor UnitTest.

Greets
Blun78

## Purpose ##

ASP.NET MVC 5 shipped with a new Identity system (in the Microsoft.AspNet.Identity.Core package) in order to support both local login and remote logins via OpenID/OAuth, but only ships with an
Entity Framework provider (Microsoft.AspNet.Identity.EntityFramework).

## Features ##
* Drop-in replacement ASP.NET Identity with RavenDB as the backing store.
* Requires only 2 document types and 3 Index, while EntityFramework requires 5 tables
* Contains the same IdentityUser class used by the EntityFramework provider in the MVC 5 project template.
* Supports in RavenDB 'string' or 'int' for Document IDs 
* Supports additional profile properties on your application's user model.
* Provides UserStore<TUser, TRole, TKey>
    * IUserStore<TUser, TKey>
    * IUserLoginStore<TUser, TKey>
    * IUserLockoutStore<TUser, TKey>
    * IUserRoleStore<TUser, TKey>
    * IUserClaimStore<TUser, TKey>
    * IUserPasswordStore<TUser, TKey>
    * IUserSecurityStampStore<TUser, TKey>
    * IUserTwoFactorStore<TUser, TKey>
    * IUserEmailStore<TUser, TKey>
    * IUserPhoneNumberStore<TUser, TKey>
    * IQueryableUserStore<TUser, TKey>
* Provide RoleStore<TRole>
    * IRoleStore<TRole, TKey>
    * IQueryableRoleStore<TRole, TKey>

## Instructions ##
These instructions assume you know how to set up RavenDB within an MVC application.

1. Create a new ASP.NET MVC 5 project, choosing the Individual User Accounts authentication type.
2. Remove the Entity Framework packages and replace with RavenDB Identity:

```PowerShell
Uninstall-Package Microsoft.AspNet.Identity.EntityFramework
Uninstall-Package EntityFramework
Install-Package Blun.AspNet.Identity.RavenDb
```
    
3. In ~/Models/IdentityModels.cs:
    * Remove the namespace: Microsoft.AspNet.Identity.EntityFramework
    * Add the namespace: RavenDB.AspNet.Identity
    * Remove the entire ApplicationDbContext class. You don't need that!
4. In ~/Controllers/AccountController.cs
    * Remove the namespace: Microsoft.AspNet.Identity.EntityFramework
    * Replace the UserStore in the constructor, providing a lambda expression to show the UserStore how to get the current IDocumentSession.

```C#
// This example assumes you have a RavenController base class with public RavenSession property.
public AccountController()
{
    using (IDocumentSession ravenSession = store.OpenSession())
    {
        ApplicationUser user = new ApplicationUser()
        {
            Name = "Demo"
        };

        ravenSession.Store(user);

        this.UserManager = new UserManager<ApplicationUser>(
                                new UserStore<ApplicationUser>(() => ravenSession));

        var result = UserManager.Create(user, "passw0rd");
        if (result.Succeeded)
        {
            ravenSession.SaveChanges();
        }
    }
}
```
