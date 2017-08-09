using Autofac;
using Isidore.MagicMirror.ImageProcessing.FaceRecognition.Classifiers;
using Isidore.MagicMirror.ImageProcessing.FaceRecognition.Services;
using Isidore.MagicMirror.Infrastructure.Exceptions;
using Isidore.MagicMirror.Infrastructure.Extensions;
using Isidore.MagicMirror.Users.Services;
using Microsoft.Extensions.FileProviders;
using MongoDB.Bson;
using MongoDB.Driver;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Bootstrappers.Autofac;
using System;
using System.Reflection;
using Isidore.MagicMirror.DAL.MongoDB.Configuration;
using Isidore.MagicMirror.Users.API.Configuration;
using Microsoft.Extensions.Configuration;

namespace Isidore.MagicMirror.Users.API
{
    public class Bootstrapper : AutofacNancyBootstrapper
    {
        private readonly IConfiguration _appConfig;

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
            // Perform registration that should have an application lifetime

            //SetLocalFaceService(container);
            container.Update(builder =>
            {
                builder.RegisterType<AzureFaceRecognitionService>().AsImplementedInterfaces();
                builder.RegisterInstance(SetUpMongoDb());
                builder.RegisterType<UserService>().AsImplementedInterfaces();
                builder.RegisterInstance(this._appConfig.Get<FaceServiceConfig>());
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

        private void SetLocalFaceService(ILifetimeScope container)
        {
            var fileProvider = new EmbeddedFileProvider(Assembly.GetEntryAssembly());
            var config = this._appConfig.GetSettings<FaceServiceConfig>();

            container.Update(builder => builder.RegisterInstance(fileProvider).As<IFileProvider>());
            container.Update(builder => builder.RegisterType<HaarCascadeClassifier>().AsImplementedInterfaces()
                .WithParameter(new NamedParameter("cascadeFileName",
                    "Assets.HaarClassifiers.haarcascade_frontalface_default.xml")));
            container.Update(builder => builder.RegisterType<FisherFaceByteProxy>().AsImplementedInterfaces()
                .WithParameter(new NamedParameter("fileName", config.LearnFile)));
        }

        private IMongoDatabase SetUpMongoDb()
        {
            IMongoDatabase mongoDb;
            var config = _appConfig.GetSettings<MongoDbConfig>();
            var credential = MongoCredential.CreateCredential(config.DbName, config.Username, config.Password);
            try
            {
                mongoDb = new MongoClient(new MongoClientSettings
                {
                    Servers = new[] { new MongoServerAddress(config.ServerUrl) },
                    ConnectTimeout = TimeSpan.FromSeconds(5),
                    //Credentials = new[] { credential }
                }).GetDatabase(config.DbName);
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
