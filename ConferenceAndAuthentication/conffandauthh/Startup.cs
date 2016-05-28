using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(conffandauthh.Startup))]
namespace conffandauthh
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
