using System;
using Blun.AspNet.Identity.RavenDB.Common;
using Microsoft.AspNet.Identity;

// ReSharper disable once CheckNamespace
namespace RavenDB.AspNet.Identity
{
    public sealed class IdentityUserLogin : IdentityUserLogin<string>
    {
        public IdentityUserLogin()
        {
        }
    }

    public sealed class IdentityUserLoginIntKey : IdentityUserLogin<int>
    {
        public IdentityUserLoginIntKey()
        {
        }
    }

    public abstract class IdentityUserLogin<TKey> : GenericBase<TKey>
        where TKey : IConvertible, IComparable, IEquatable<TKey>
    {
        public string Id { get; set; }
        public TKey UserId { get; set; }
        public UserLoginInfo UserLoginInfo { get; set; }

        protected IdentityUserLogin()
            : base()
        {
        }

        protected IdentityUserLogin(string loginProvider, string providerKey)
            : this()
        {
            base.CheckArgumentForNull(loginProvider, "loginProvider");
            base.CheckArgumentForNull(providerKey, "providerKey");

            UserLoginInfo = new UserLoginInfo(loginProvider, providerKey);
        }

        internal static IdentityUserLogin<TKey> CreateIdentityUserLogin()
        {
            if (CheckInt())
            {
                return (new IdentityUserLoginIntKey()) as IdentityUserLogin<TKey>;
            }
            else if (CheckString())
            {
                return (new IdentityUserLogin()) as IdentityUserLogin<TKey>;
            }
            else
            {
                ThrowTypeAccessException(typeof(TKey));
            }
            return null;
        }
    }
}