using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Sample.Startup))]
namespace Sample
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
