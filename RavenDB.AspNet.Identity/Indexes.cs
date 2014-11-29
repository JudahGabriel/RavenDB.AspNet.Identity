using System;
using System.Linq;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace RavenDB.AspNet.Identity
{
    public class UserByUserNameIndex<TUser> : AbstractIndexCreationTask<TUser> 
        where TUser : IdentityUser
    {
        public UserByUserNameIndex()
        {
            Map = users => from user in users
                              select new
                              {
                                  UserName = user.UserName,
                                  Id = user.Id
                              };

            Index(x => x.UserName, FieldIndexing.Default);
            Index(x => x.Id, FieldIndexing.Default);
            Store(x => x.UserName, FieldStorage.Yes);
        }
    }
}
