using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace RavenDB.AspNet.Identity.Index
{
    internal class IdentityUser_GetByEmail<TUser, TKey> : AbstractIndexCreationTask<TUser>
        where TUser : IdentityUser<TKey>
        where TKey : IConvertible, IComparable, IEquatable<TKey>
    {
        public class Result
        {
            public TKey Id { get; set; }
            public string Email { get; set; }
        }

        public IdentityUser_GetByEmail()
        {
            Index(x => x.Email, FieldIndexing.Default);

            Map = users => users.Select(user => new Result()
                {
                    Id = user.Id,
                    Email = user.Email
                });
        }
    }
}