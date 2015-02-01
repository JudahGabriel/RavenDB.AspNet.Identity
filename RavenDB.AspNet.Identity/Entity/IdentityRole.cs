using System;
using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using RavenDB.AspNet.Identity.Common;

// ReSharper disable once CheckNamespace
namespace RavenDB.AspNet.Identity
{
    public class IdentityRole : IdentityRole<string>, IRole
    {
        public IdentityRole()
            : base()
        {
        }
    }

    public class IdentityRoleIntKey : IdentityRole<int>
    {
        public IdentityRoleIntKey()
            : base()
        {
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public abstract class IdentityRole<TKey> : 
                                        GenericBase<TKey>, 
                                        IRole<TKey>
        where TKey : IConvertible, IComparable, IEquatable<TKey>
    {
        public IdentityRole()
            : base()
        {
        }

        public TKey Id { get; set; }
        public string Name { get; set; }
        public List<TKey> UserIds { get; set; }
    }
}