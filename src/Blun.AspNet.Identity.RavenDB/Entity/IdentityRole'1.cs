using System;
using System.Collections.Generic;
using Blun.AspNet.Identity.RavenDB.Common;
using Microsoft.AspNet.Identity;

// ReSharper disable once CheckNamespace
namespace RavenDB.AspNet.Identity
{
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