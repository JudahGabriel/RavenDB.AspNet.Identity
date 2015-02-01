using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Raven.Imports.Newtonsoft.Json;
using RavenDB.AspNet.Identity.Common;
using RavenDB.AspNet.Identity.Entity;


// ReSharper disable once CheckNamespace
namespace RavenDB.AspNet.Identity
{
    public class IdentityUser : IdentityUser<string>, IUser
    {
        public IdentityUser()
            : base()
        {
        }
    }

    public class IdentityUserIntKey : IdentityUser<int>
    {
        public IdentityUserIntKey()
            : base()
        {
        }
    }

    public class IdentityUser<TKey> :
                                         GenericBase<TKey>,
                                         IUser<TKey>
        where TKey : IConvertible, IComparable, IEquatable<TKey>
    {
        #region IUser

        public TKey Id { get; set; }
        public string UserName { get; set; }

        #endregion

        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public bool IsPhoneNumberConfirmed { get; set; }
        public int AccessFailedCount { get; set; }
        public bool LockoutEnabled { get; set; }
        public DateTimeOffset LockoutEndDate { get; set; }
        public bool TwoFactorAuthEnabled { get; set; }

        public List<TKey> Roles { get; set; }
        public List<IdentityUserClaim> Claims { get; set; }
        public List<IdentityUserLogin<TKey>> Logins { get; set; }

        public IdentityUser()
            : base()
        {
            SecurityStamp = Guid.NewGuid().ToString();
            this.Roles = new List<TKey>();
            this.Claims = new List<IdentityUserClaim>();
            this.Logins = new List<IdentityUserLogin<TKey>>();
        }
    }
}
