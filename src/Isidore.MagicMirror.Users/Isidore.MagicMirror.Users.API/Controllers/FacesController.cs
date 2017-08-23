using System;
using Isidore.MagicMirror.ImageProcessing.FaceRecognition.Services;
using Nancy;
using Nancy.ModelBinding;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Isidore.MagicMirror.ImageProcessing.FaceRecognition.Exceptions;
using Isidore.MagicMirror.Users.Contract;
using Isidore.MagicMirror.Users.Models;
using Isidore.MagicMirror.WebService.Http.FileUploads;
using NLog;

namespace Isidore.MagicMirror.Users.API.Controllers
{
    public class FacesController : NancyModule
    {
        private readonly IFaceRecognitionService<Stream, User> _faceService;
        private readonly IUserService _usersService;
        private readonly Stopwatch _watch = new Stopwatch();
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public FacesController(IUserService userService, IFaceRecognitionService<Stream, User> faceService) : base("/faces")
        {
            _faceService = faceService;
            _usersService = userService;
            RegisterActions();
        }

        private void RegisterActions()
        {
            Post("/learn/{id}", async (_, ctx) =>
            {
                var request = this.Bind<FileUploadRequest>();
                if (!ValidateRequest(request))
                {
                    return Negotiate.WithStatusCode(400);
                }

                return await LearnImage(request, _["id"]);
            });

            Post("/recognize", async _ =>
            {
                var request = this.Bind<FileUploadRequest>();
                if (!ValidateRequest(request))
                {
                    return Negotiate.WithStatusCode(400);
                }

                return await RecognizeUser(request);
            });
        }

        private bool ValidateRequest(FileUploadRequest request)
        {
            return request.File != null;
        }

        public async Task<dynamic> LearnImage(FileUploadRequest response, string id)
        {
            _watch.Reset();
            _watch.Start();
            if (!response.File.Name.ToLowerInvariant().EndsWith(".jpg"))
            {
                var r = (Response)"The file doesn't have not .jpg extension";
                r.StatusCode = HttpStatusCode.BadRequest;
                return r;
            }
            var imageBytes = response.File.Value;
            User user;
            try
            {
                user = _usersService.GetById(id);
                if (user == null)
                {
                    return Negotiate.WithStatusCode(404);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "Error on getting user from storage.");
                return Negotiate.WithStatusCode(500);
            }

            var usersToLearn = new Dictionary<User, IEnumerable<Stream>>();
            usersToLearn.Add(user, new List<Stream> { imageBytes });
            try
            {
                await _faceService.LearnMore(usersToLearn);
            }
            catch (FaceNotFoundException e)
            {
                return Response.AsJson(e.Message).WithStatusCode(HttpStatusCode.BadRequest);
            }
            _watch.Stop();
            return $"Learned {id} with {imageBytes.Length} bytes in {_watch.ElapsedMilliseconds} ms";
        }

        public async Task<dynamic> RecognizeUser(FileUploadRequest request)
        {
            _watch.Reset();
            _watch.Start();
            if (!request.File.Name.ToLowerInvariant().EndsWith(".jpg"))
            {
                var r = (Response)"The file doesn't have not .jpg extension";
                r.StatusCode = HttpStatusCode.BadRequest;
                return r;
            }
            var u = await _faceService.RecognizeAsync(request.File.Value);

            _watch.Stop();
            if (u == null)
            {
                return await Negotiate.WithStatusCode(204).WithModel(new
                {
                    error = "No face found"
                });
            }
            else if (u.RecognizedItem == null)
            {
                return await Negotiate.WithStatusCode(404).WithModel(new
                {
                    error = "The face can't be assigned",
                    area = u.Area
                });
            }

            return u;
        }
    }
}
