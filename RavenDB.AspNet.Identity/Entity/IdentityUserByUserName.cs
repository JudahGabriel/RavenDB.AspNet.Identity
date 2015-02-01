using System;
using RavenDB.AspNet.Identity.Common;

namespace RavenDB.AspNet.Identity.Entity
{
    internal sealed class IdentityUserByUserName<TKey> : GenericBase<TKey>
        where TKey : IConvertible, IComparable, IEquatable<TKey>
    {
        public string Id { get; set; }
        public TKey UserId { get; set; }
        public string UserName { get; set; }

        public IdentityUserByUserName(TKey userId, string userName) : this()
        {
            base.CheckArgumentForNull(userId, "userId");
            base.CheckArgumentForNull(userName, "userName");

            UserId = userId;
            UserName = userName;
        }

        public IdentityUserByUserName() : base()
        {
        }
    }
}