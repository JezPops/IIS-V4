using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(IIS_V4.Areas.Identity.IdentityHostingStartup))]
namespace IIS_V4.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
            });
        }
    }
}