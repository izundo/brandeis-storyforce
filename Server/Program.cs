using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace StoryForce.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    //webBuilder.UseSentry(options =>
                    //{
                    //    //Disable Sentry Debug message
                    //    options.Debug = false;
                    //});
                    webBuilder.UseStartup<Startup>();
                });
    }
}