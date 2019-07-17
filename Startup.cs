using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DTransAPI.Startup))]
namespace DTransAPI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
