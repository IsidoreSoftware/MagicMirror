using Isidore.MagicMirror.ImageProcessing.FaceRecognition.Classifiers;
using Isidore.MagicMirror.ImageProcessing.FaceRecognition.Services;
using Nancy;
using Nancy.ModelBinding;
using Isidore.MagicMirror.Infrastructure.Http.FileUploads;
using Isidore.MagicMirror.Utils.Helpers.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using System.Reflection;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using Isidore.MagicMirror.Users.Models;
using System.Linq;
using Isidore.MagicMirror.Infrastructure.Services;

namespace Isidore.MagicMirror.API.Controllers
{
    public class FacesController : NancyModule
    {
        private IFaceRecognitionService<byte[], Person> _faceService;
        private IDataService<Person> _usersService;
        private Stopwatch watch = new Stopwatch();

        private const string LearnFilePath = "D:\\Kuba\\Desktop\\learn.yml";

        public FacesController() : base("/faces")
        {
            var fileProvider = new EmbeddedFileProvider(Assembly.GetEntryAssembly());

            var classifier = new HaarCascadeClassifier(fileProvider,
                "Assets.HaarClassifiers.haarcascade_frontalface_default.xml");
            _faceService = new FisherFaceByteProxy(classifier, LearnFilePath);
            //_usersService = new UserService();
            RegisterActions();
        }

        private void RegisterActions()
        {

            base.Post("/learn/{id}", async (_, ctx) =>
            {
                var response = this.Bind<FileUploadRequest>();
                return await LearnImage(response, _["id"]);
            });

            Post("/recognize", async _ =>
            {
                var response = this.Bind<FileUploadRequest>();
                return await RecognizeUser(response);
            });
        }

        public async Task<dynamic> LearnImage(FileUploadRequest response, string id)
        {
            watch.Reset();
            watch.Start();
            if (!response.File.Name.EndsWith(".jpg"))
            {
                var r = (Response)"The file doesn't have not .jpg extension";
                r.StatusCode = HttpStatusCode.BadRequest;
                return r;
            }
            var imageBytes = await response.File.Value.ToByteArray();
            var user = _usersService.GetById(id);
            var usersToLearn = new Dictionary<Person, IEnumerable<byte[]>>();
            usersToLearn.Add(user, new List<byte[]> { imageBytes });
            await _faceService.LearnMore(usersToLearn, LearnFilePath);
            watch.Stop();
            return $"Learned {id} with {imageBytes.Length} bytes in {watch.ElapsedMilliseconds} ms";
        }

        public async Task<dynamic> RecognizeUser(FileUploadRequest response)
        {

            watch.Reset();
            watch.Start();
            if (!response.File.Name.EndsWith(".jpg"))
            {
                var r = (Response)"The file doesn't have not .jpg extension";
                r.StatusCode = HttpStatusCode.BadRequest;
                return r;
            }
            var imageBytes = await response.File.Value.ToByteArray();
            var users = _usersService.GetAll();
            var u = await _faceService.RecognizeAsync(imageBytes, users.ToList(), LearnFilePath);

            watch.Stop();
            return $"It's {u.RecognizedItem.Name} (d:{u.Distance:#.##}). Recognized in {watch.ElapsedMilliseconds} ms";
        }
    }
}
