using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ImageSelectionServer
{
    public class Startup
    {
        public static Dictionary<string, string> dictionary = new Dictionary<string, string>();

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private static void Initial()
        {
            Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\Images\\");
            var dic = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "\\fa.json");
            dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(dic);

            var words = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "\\Images\\", "*.*");
            var downloadedWords = words.Select(x =>
                x.Split('\\').LastOrDefault()
                    ?.Split(new string[] { ".jpg" }, StringSplitOptions.RemoveEmptyEntries)
                    .FirstOrDefault()).ToList();

            foreach (var word in downloadedWords)
                dictionary.Remove(word.Trim());

            string[] skipedWords = null;
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\Skiped.txt"))
                skipedWords = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "\\Skiped.txt");
            if (skipedWords != null)
                foreach (var word in skipedWords)
                    dictionary.Remove(word.Trim());

        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
