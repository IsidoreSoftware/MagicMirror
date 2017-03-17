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

namespace Isidore.MagicMirror.Widgets.API
{
    public class Bootstrapper : AutofacNancyBootstrapper
    {
        private const string LearnFilePath = "\\learn.yml";
        private const string MongoDbName = "magic-mirror";

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
        }

        private static IMongoDatabase SetUpMongoDb()
        {
            IMongoDatabase mongoDb;
            var credential = MongoCredential.CreateCredential(MongoDbName, "admin", "");
            try
            {
                mongoDb = new MongoClient(new MongoClientSettings
                {
                    Servers = new[] { new MongoServerAddress("mongo-db") },
                    ConnectTimeout = TimeSpan.FromSeconds(5)
                }).GetDatabase(MongoDbName);
                mongoDb.RunCommandAsync((Command<BsonDocument>)"{ping:1}")
                 .Wait();
            }
            catch (Exception e)
            {
                throw new DependentComponentException(ComponentType.MongoDb, e, $"Database name: {MongoDbName}");
            }

            return mongoDb;
        }
    }
}
