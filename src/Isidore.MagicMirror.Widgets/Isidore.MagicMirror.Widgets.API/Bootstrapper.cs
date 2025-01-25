using Autofac;
using Isidore.MagicMirror.Infrastructure.Exceptions;
using Isidore.MagicMirror.Widgets.Contract;
using Isidore.MagicMirror.Widgets.Services;
using MongoDB.Bson;
using MongoDB.Driver;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Bootstrappers.Autofac;
using System;
using Isidore.MagicMirror.DAL.MongoDB.Configuration;
using Isidore.MagicMirror.Infrastructure.Extensions;
using Microsoft.Extensions.Configuration;
using System.Runtime.InteropServices;

namespace Isidore.MagicMirror.Widgets.API
{
    public class Bootstrapper : AutofacNancyBootstrapper
    {
        private IConfiguration _appConfig;

        public Bootstrapper(IConfiguration appConfig)
        {
            _appConfig = appConfig;
        }

        protected override void ApplicationStartup(ILifetimeScope container, IPipelines pipelines)
        {
            // No registrations should be performed in here, however you may
            // resolve things that are needed during application startup.
        }

        protected override void ConfigureApplicationContainer(ILifetimeScope container)
        {
            IMongoDatabase mongoDb = SetUpMongoDb();

            // Perform registration that should have an application lifetime
            container.Update(builder => builder.RegisterInstance(mongoDb).As<IMongoDatabase>());
            container.Update(builder => builder.RegisterType<WidgetService>().As<IWidgetService>());
        }

        protected override void ConfigureRequestContainer(ILifetimeScope container, NancyContext context)
        {
            // Perform registrations that should have a request lifetime
        }

        protected override void RequestStartup(ILifetimeScope container, IPipelines pipelines, NancyContext context)
        {
            // No registrations should be performed in here, however you may
            // resolve things that are needed during request startup.
            pipelines.AfterRequest.AddItemToEndOfPipeline((ctx) =>
            {
                ctx.Response.WithHeader("Access-Control-Allow-Origin", "*")
                                .WithHeader("Access-Control-Allow-Methods", "POST,GET")
                                .WithHeader("Access-Control-Allow-Headers", "Accept, Origin, Content-type");

            });
        }

        private IMongoDatabase SetUpMongoDb()
        {
            IMongoDatabase mongoDb;
            var config = _appConfig.GetSettings<MongoDbConfig>();
            try
            {
                mongoDb = new MongoClient(config.ConnectionString)
                    .GetDatabase(config.DbName);
                mongoDb.RunCommandAsync((Command<BsonDocument>)"{ping:1}")
                    .Wait();
            }
            catch (Exception e)
            {
                throw new DependentComponentException(ComponentType.MongoDb, e, $"Database name: {config.DbName}");
            }

            return mongoDb;
        }
    }
}
