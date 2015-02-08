using System;
using System.Linq;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;
using RavenDB.AspNet.Identity;

namespace Blun.AspNet.Identity.RavenDB.Index
{
    internal class IdentityRole_GetByName<TRole, TKey> : AbstractIndexCreationTask<TRole>
        where TRole : IdentityRole<TKey>
        where TKey : IConvertible, IComparable, IEquatable<TKey>
    {
        public class Result
        {
            public TKey Id { get; set; }
            public string Name { get; set; }
        }
        
        public IdentityRole_GetByName()
        {
            Index(x => x.Name, FieldIndexing.Default);
            
            Map = roles => roles.Select(
                role => new Result()
                {
                    Id = role.Id,
                    Name = role.Name
                });
        }
    }
}
