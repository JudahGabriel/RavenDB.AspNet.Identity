# RavenDB.AspNetCore.Identity #
An ASP.NET Core Identity provider for RavenDB

## Instructions ##
1. In Startup.cs:

	// Add RavenDB and identity.
    services
		.AddRavenDb(Configuration.GetConnectionString("RavenDbConnection")) // Create a RavenDB DocumentStore singleton.
        .AddRavenDbAsyncSession() // Create a RavenDB IASyncDocumentSession for each request.
        .AddRavenDbIdentity<AppUser>(); // Use Raven for users and roles.

2. Call .SaveChanges when you're done making changes. Typically this is done via a RavenController base class. For example: https://github.com/JudahGabriel/RavenDB.AspNet.Identity/blob/master/Sample.Web.NetCore/Controllers/RavenController.cs

3. You're done!