using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MD2241A5.Startup))]

namespace MD2241A5
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
