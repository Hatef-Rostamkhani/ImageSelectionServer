using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.WindowsServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace ImageSelectionServer
{
    public class Program
    {
        public static void Main(string[] args)
        {



            var pfx = new X509Certificate2(AppDomain.CurrentDomain.BaseDirectory + "\\hatef.co.pfx", "123456789");
            var builder = CreateWebHostBuilder(
                args.Where(arg => arg != "--console").ToArray());
            var host = builder
                 //.UseConfiguration(config)
                 .UseKestrel(options =>
                 {
                     options.Listen(IPAddress.Any, 443, listenOptions => { listenOptions.UseHttps(pfx); });
                 })
                 .UseUrls("http://*:5000", "http://localhost:5000", "https://*:5001", "https://*:443")
                 .UseContentRoot(Directory.GetCurrentDirectory())
                 .UseIISIntegration()
                 .UseStartup<Startup>()
                 .Build();

            var isService = !(Debugger.IsAttached || args.Contains("--console"));

            if (isService)
            {
                var processModule = Process.GetCurrentProcess().MainModule;
                if (processModule != null)
                {
                    var pathToExe = processModule.FileName;
                    var pathToContentRoot = Path.GetDirectoryName(pathToExe);
                    Directory.SetCurrentDirectory(pathToContentRoot);
                }
            }




            if (isService)
            {
                // To run the app without the CustomWebHostService change the
                // next line to
                host.RunAsService();
                //host.RunAsCustomService();
            }
            else
            {
                host.Run();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddEventLog();
                })
                .ConfigureAppConfiguration((context, config) =>
                {
                    // Configure the app here.
                })
                .UseStartup<Startup>();
    }
    public static class WebHostServiceExtensions
    {
        public static void RunAsCustomService(this IWebHost host)
        {
            var webHostService = new CustomWebHostService(host);
            ServiceBase.Run(webHostService);
        }
    }
    public class CustomWebHostService : WebHostService
    {
        private ILogger _logger;

        public CustomWebHostService(IWebHost host) : base(host)
        {
            _logger = host.Services
                .GetRequiredService<ILogger<CustomWebHostService>>();
        }

        protected override void OnStarting(string[] args)
        {
            _logger.LogInformation("OnStarting method called.");
            base.OnStarting(args);
        }

        protected override void OnStarted()
        {
            _logger.LogInformation("OnStarted method called.");
            base.OnStarted();
        }

        protected override void OnStopping()
        {
            _logger.LogInformation("OnStopping method called.");
            base.OnStopping();
        }
    }
}
