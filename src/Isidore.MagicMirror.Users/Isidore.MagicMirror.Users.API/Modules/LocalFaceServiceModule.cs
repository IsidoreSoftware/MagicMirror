using System.Reflection;
using Autofac;
using Isidore.MagicMirror.ImageProcessing.FaceRecognition.Classifiers;
using Isidore.MagicMirror.ImageProcessing.FaceRecognition.Services;
using Isidore.MagicMirror.Infrastructure.Extensions;
using Isidore.MagicMirror.Users.API.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Module = Autofac.Module;

namespace Isidore.MagicMirror.Users.API.Modules
{
    public class LocalFaceServiceModule : Module
    {
        private readonly IConfiguration _appConfig;

        public LocalFaceServiceModule(IConfiguration appConfig)
        {
            _appConfig = appConfig;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var fileProvider = new EmbeddedFileProvider(Assembly.GetEntryAssembly());
            var config = _appConfig.GetSettings<FaceServiceConfig>();

            builder.RegisterInstance(fileProvider).As<IFileProvider>();
            builder.RegisterType<HaarCascadeClassifier>().AsImplementedInterfaces()
                .WithParameter(new NamedParameter("cascadeFileName",
                    "Assets.HaarClassifiers.haarcascade_frontalcatface_extended.xml"));
            builder.RegisterType<FisherFaceByteProxy>().AsImplementedInterfaces()
                .WithParameter(new NamedParameter("fileName", config.LearnFile));
        }
    }
}
