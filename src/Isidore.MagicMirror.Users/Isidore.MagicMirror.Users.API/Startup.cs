using System;
using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nancy.Owin;
using NLog.Extensions.Logging;
using NLog.Web;

namespace Isidore.MagicMirror.Users.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; set; }

        public Startup(IHostingEnvironment env)
        {
            env.ConfigureNLog("nlog.config");
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>();
            }

            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            SetupLogger(app, loggerFactory);
            UseNancy(app, loggerFactory);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }

        private static void SetupLogger(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole().AddNLog();
            //add NLog.Web
            app.AddNLogWeb();
        }

        private void UseNancy(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            try
            {
                app.UseOwin(pipe => pipe.UseNancy(x => x.Bootstrapper = new Bootstrapper(Configuration, loggerFactory)));
            }
            catch (Exception e)
            {
                Exception firstException = e;
                while (firstException.InnerException != null)
                {
                    firstException = firstException.InnerException;
                }

                Console.Error.WriteLine(firstException.Message);

                throw;
            }
        }
    }
}
