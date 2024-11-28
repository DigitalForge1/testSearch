using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace alphaData
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var bulder = CreateHostBuilder(args).Build();
            bulder.Run();

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
