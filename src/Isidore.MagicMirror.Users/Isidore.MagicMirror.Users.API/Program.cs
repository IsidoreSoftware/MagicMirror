using System.IO;
using Microsoft.AspNetCore.Hosting;
using NLog.Web;

namespace Isidore.MagicMirror.Users.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .UseNLog()
                .Build();

            host.Run();
        }
    }
}
