using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Enrampage.Startup))]
namespace Enrampage
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder App)
        {
            ConfigureAuth(App);
        }
    }
}