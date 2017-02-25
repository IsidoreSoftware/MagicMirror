using Autofac;
using Isidore.MagicMirror.ImageProcessing.FaceRecognition.Classifiers;
using Isidore.MagicMirror.ImageProcessing.FaceRecognition.Services;
using Isidore.MagicMirror.Users.Contract;
using Isidore.MagicMirror.Users.Services;
using Microsoft.Extensions.FileProviders;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Bootstrappers.Autofac;
using System.Reflection;

namespace Isidore.MagicMirror.API
{
    public class Bootstrapper : AutofacNancyBootstrapper
    {
        private const string LearnFilePath = "D:\\Kuba\\Desktop\\learn.yml";

        protected override void ApplicationStartup(ILifetimeScope container, IPipelines pipelines)
        {
            // No registrations should be performed in here, however you may
            // resolve things that are needed during application startup.
        }

        protected override void ConfigureApplicationContainer(ILifetimeScope container)
        {
            // Perform registration that should have an application lifetime
            var fileProvider = new EmbeddedFileProvider(Assembly.GetEntryAssembly());
            container.Update(builder => builder.RegisterType<UserService>().AsImplementedInterfaces());
            container.Update(builder => builder.RegisterInstance(fileProvider).As<IFileProvider>());
            container.Update(builder => builder.RegisterType<HaarCascadeClassifier>().AsImplementedInterfaces()
                .WithParameter(new NamedParameter("cascadeFileName", "Assets.HaarClassifiers.haarcascade_frontalface_default.xml")));
            container.Update(builder => builder.RegisterType<FisherFaceByteProxy>().AsImplementedInterfaces()
                .WithParameter(new NamedParameter("",LearnFilePath)));
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
