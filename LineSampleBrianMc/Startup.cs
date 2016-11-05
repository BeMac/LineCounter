using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(LineSampleBrianMc.Startup))]
namespace LineSampleBrianMc
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
