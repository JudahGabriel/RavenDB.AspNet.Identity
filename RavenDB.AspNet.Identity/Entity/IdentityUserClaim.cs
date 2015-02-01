using System.Security.Claims;

namespace Blun.AspNet.Identity.RavenDB.Entity
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