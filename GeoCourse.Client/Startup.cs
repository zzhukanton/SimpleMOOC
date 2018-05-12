using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GeoCourse.Client.Startup))]
namespace GeoCourse.Client
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
