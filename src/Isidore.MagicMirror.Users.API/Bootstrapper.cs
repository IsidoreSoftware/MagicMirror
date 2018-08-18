using Autofac;
using Isidore.MagicMirror.ImageProcessing.FaceRecognition.Classifiers;
using Isidore.MagicMirror.ImageProcessing.FaceRecognition.Services;
using Isidore.MagicMirror.Users.API.Exceptions;
using Isidore.MagicMirror.Users.Services;
using Microsoft.Extensions.FileProviders;
using MongoDB.Bson;
using MongoDB.Driver;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Bootstrappers.Autofac;
using System;
using System.Reflection;

namespace Isidore.MagicMirror.Users.API
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
            var fileProvider = new EmbeddedFileProvider(Assembly.GetEntryAssembly());
            container.Update(builder => builder.RegisterInstance(mongoDb));
            container.Update(builder => builder.RegisterType<UserService>().AsImplementedInterfaces());
            container.Update(builder => builder.RegisterType<UserService>().AsImplementedInterfaces());
            container.Update(builder => builder.RegisterInstance(fileProvider).As<IFileProvider>());
            container.Update(builder => builder.RegisterType<HaarCascadeClassifier>().AsImplementedInterfaces()
                .WithParameter(new NamedParameter("cascadeFileName", "Assets.HaarClassifiers.haarcascade_frontalface_default.xml")));
            container.Update(builder => builder.RegisterType<FisherFaceByteProxy>().AsImplementedInterfaces()
                .WithParameter(new NamedParameter("fileName", LearnFilePath)));
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
                    Servers = new[] {new MongoServerAddress("mongo-db") },
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
