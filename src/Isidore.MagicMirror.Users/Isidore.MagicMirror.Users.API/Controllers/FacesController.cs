using Isidore.MagicMirror.ImageProcessing.FaceRecognition.Services;
using Nancy;
using Nancy.ModelBinding;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;
using Isidore.MagicMirror.Users.Contract;
using Isidore.MagicMirror.Users.Models;
using Isidore.MagicMirror.WebService.Http.FileUploads;
using Isidore.MagicMirror.Utils.Helpers.IO;

namespace Isidore.MagicMirror.Users.API.Controllers
{
    public class FacesController : NancyModule
    {
        private readonly IFaceRecognitionService<byte[],User> _faceService;
        private readonly IUserService _usersService;
        private readonly Stopwatch watch = new Stopwatch();

        public FacesController(IUserService userService, IFaceRecognitionService<byte[], User> faceService) : base("/faces")
        {
            _faceService = faceService;
            _usersService = userService;
            RegisterActions();
        }

        private void RegisterActions()
        {
            Post("/learn/{id}", async (_, ctx) =>
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
            var usersToLearn = new Dictionary<User, IEnumerable<byte[]>>();
            usersToLearn.Add(user, new List<byte[]> { imageBytes });
            await _faceService.LearnMore(usersToLearn);
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
            var u = await _faceService.RecognizeAsync(imageBytes);

            watch.Stop();
            return $"It's {u.RecognizedItem.ToString()} (d:{u.Distance:#.##}). Recognized in {watch.ElapsedMilliseconds} ms";
        }
    }
}
