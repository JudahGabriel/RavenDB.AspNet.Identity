using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using RavenDB.AspNet.Identity;

namespace Blun.AspNet.Identity.RavenDB
{
    public class IdentityRole : IdentityRole<string>, IRole
    {
        public IdentityRole()
            : base()
        {
        }
    }

}
