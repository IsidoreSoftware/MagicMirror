using Autofac;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Bootstrappers.Autofac;
using Isidore.MagicMirror.Users.API.Modules;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Isidore.MagicMirror.Users.API
{
    public class Bootstrapper : AutofacNancyBootstrapper
    {
        private readonly IConfiguration _appConfig;
        private readonly ILoggerFactory _loggerFactory;

        public Bootstrapper(IConfiguration appConfig, ILoggerFactory loggerFactory)
        {
            _appConfig = appConfig;
            _loggerFactory = loggerFactory;
        }

        protected override void ApplicationStartup(ILifetimeScope container, IPipelines pipelines)
        {
            // No registrations should be performed in here, however you may
            // resolve things that are needed during application startup.
        }

        protected override void ConfigureApplicationContainer(ILifetimeScope container)
        {
            // Perform registration that should have an application lifetime

            container.Update(builder =>
            {
                var usersModule = new UsersModule(_appConfig, _loggerFactory);
                builder.RegisterModule(usersModule);
                builder.RegisterModule(new LocalFaceServiceModule(_appConfig));
            });
        }

        protected override void ConfigureRequestContainer(ILifetimeScope container, NancyContext context)
        {
            // Perform registrations that should have a request lifetime
        }

        protected override void RequestStartup(ILifetimeScope container, IPipelines pipelines, NancyContext context)
        {
            // No registrations should be performed in here, however you may
            // resolve things that are needed during request startup.
        }
    }
}
