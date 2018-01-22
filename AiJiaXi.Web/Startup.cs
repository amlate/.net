using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AiJiaXi.Web.Startup))]
namespace AiJiaXi.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
