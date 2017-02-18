using Isidore.MagicMirror.ImageProcessing.FaceRecognition.Classifiers;
using Isidore.MagicMirror.ImageProcessing.FaceRecognition.Services;
using Nancy;
using Nancy.ModelBinding;
using Isidore.MagicMirror.Infrastructure.Http.FileUploads;
using Isidore.MagicMirror.Utils.Helpers.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using System.Reflection;

namespace Isidore.MagicMirror.API.Controllers
{
    public class FacesController : NancyModule
    {
        private IFaceRecognitionService<byte[]> _faceService;

        public FacesController() : base("/faces")
        {
            var fileProvider = new EmbeddedFileProvider(Assembly.GetEntryAssembly());
            var classifier = new HaarCascadeClassifier(fileProvider);
            _faceService = new FisherFaceByteProxy(classifier, "D:\\Kuba\\Desktop\\learn.yml");
            RegisterActions();
        }

        private void RegisterActions()
        {

            base.Post("/learn/{id}", async (_, ctx) =>
            {
                var response = this.Bind<FileUploadRequest>();
                return await LearnImage(response, _["id"]);
            });

            Post("/recognize", _ =>
            {
                return 500;
            });
        }

        public async Task<dynamic> LearnImage(FileUploadRequest response, string id)
        {

            if (!response.File.Name.EndsWith(".jpg"))
            {
                var r = (Response)"The file doesn't have not .jpg extension";
                r.StatusCode = HttpStatusCode.BadRequest;
                return r;
            }
            var imageBytes = await response.File.Value.ToByteArray();


            return $"Learned {id} with {imageBytes.Length} bytes";
        }
    }
}
