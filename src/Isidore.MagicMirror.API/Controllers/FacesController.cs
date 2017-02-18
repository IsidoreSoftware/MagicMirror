using Isidore.MagicMirror.ImageProcessing.FaceRecognition.Classifiers;
using Isidore.MagicMirror.ImageProcessing.FaceRecognition.Services;
using Nancy;
using Nancy.ModelBinding;
using Isidore.MagicMirror.Infrastructure.Http.FileUploads;
using Isidore.MagicMirror.Utils.Helpers.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using System.Reflection;
using Isidore.MagicMirror.API.Services;
using System.Collections.Generic;
using Isidore.MagicMirror.ImageProcessing.FaceRecognition.Models;
using System;

namespace Isidore.MagicMirror.API.Controllers
{
    public class FacesController : NancyModule
    {
        private IFaceRecognitionService<byte[]> _faceService;
        private UserService _usersService;

        public FacesController() : base("/faces")
        {
            var fileProvider = new EmbeddedFileProvider(Assembly.GetEntryAssembly());
            
            var classifier = new HaarCascadeClassifier(fileProvider,
                "Assets.HaarClassifiers.haarcascade_frontalface_default.xml");
            _faceService = new FisherFaceByteProxy(classifier, "D:\\Kuba\\Desktop\\learn.yml");
            _usersService = new UserService();
            RegisterActions();
        }

        private void RegisterActions()
        {

            base.Post("/learn/{id}", async (_, ctx) =>
            {
                var response = this.Bind<FileUploadRequest>();
                try
                {
                    return await LearnImage(response, _["id"]);
                }
                catch (Exception ex)
                {
                    throw;
                }
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
            var user = _usersService.GetPersonById(id);
            var usersToLearn = new Dictionary<Person, IEnumerable<byte[]>>();
            usersToLearn.Add(user, new List<byte[]> { imageBytes });
            await _faceService.Learn(usersToLearn);
            return $"Learned {id} with {imageBytes.Length} bytes";
        }
    }
}
