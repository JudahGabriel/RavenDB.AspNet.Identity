using System;
using System.Security.Claims;
using RavenDB.AspNet.Identity.Common;

namespace RavenDB.AspNet.Identity.Entity
{
    public class IdentityUserClaim
    {

        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
       

        public IdentityUserClaim() : base()
        {
        }

        public IdentityUserClaim(Claim claim) : this()
        {
            ClaimType = claim.Type;
            ClaimValue = claim.Value;
        }
    }

}