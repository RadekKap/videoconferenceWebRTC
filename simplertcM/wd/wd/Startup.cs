using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(wd.Startup))]
namespace wd
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
