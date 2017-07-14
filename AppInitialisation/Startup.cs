using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Diagnostics;
using System.Globalization;

namespace AppInitialisation
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Include;
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                options.SerializerSettings.Culture = CultureInfo.InvariantCulture;
            });

            services.AddSingleton<Initialisation>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory
                .AddConsole()
                .AddAzureWebAppDiagnostics();

            loggerFactory.CreateLogger<Startup>().LogInformation($"{Environment.MachineName} {Process.GetCurrentProcess().Id} starting.");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Use MVC.
            app.UseMvc();

            loggerFactory.CreateLogger<Startup>().LogInformation($"{Environment.MachineName} {Process.GetCurrentProcess().Id} started.");
        }
    }
}