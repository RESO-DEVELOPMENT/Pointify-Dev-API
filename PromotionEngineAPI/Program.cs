using ApplicationCore.Worker;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace PromotionEngineAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            using var host = Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
               .ConfigureServices((hostContext, services) =>
               {
                   #region snippet3
                   services.AddSingleton<VoucherWorker>();
                   services.AddHostedService<QueuedHostedService>();
                   services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
                   #region automapper configure
                   var mapperConfig = new MapperConfiguration(mc =>
                   {
                       mc.AddProfile(new Infrastructure.AutoMapper.AutoMapper());
                   });
                   IMapper mapper = mapperConfig.CreateMapper();
                   services.AddSingleton(mapper);
                   #endregion
                   #endregion
               })
               .Build();
            await host.StartAsync();
            await host.WaitForShutdownAsync();
        }

        //public static IHostBuilder CreateHostBuilder(string[] args) =>
        //    Host.CreateDefaultBuilder(args)
        //        .ConfigureWebHostDefaults(webBuilder =>
        //        {
        //            webBuilder.UseStartup<Startup>();
        //        })
        //        .ConfigureServices(services =>
        //        {
        //            services.AddSingleton<MonitorLoop>();
        //            services.AddHostedService<QueuedHostedService>();
        //            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
        //        });
    }
}
