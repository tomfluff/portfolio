using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(SunnyDay.Backend.Startup))]

namespace SunnyDay.Backend
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}