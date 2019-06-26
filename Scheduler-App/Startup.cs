using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Scheduler_App.Startup))]
namespace Scheduler_App
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
