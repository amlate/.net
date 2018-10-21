using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Project.Web.Startup))]
namespace Project.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
