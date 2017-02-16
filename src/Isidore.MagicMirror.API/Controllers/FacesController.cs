using System;
using Isidore.MagicMirror.ImageProcessing.FaceRecognition;
using Nancy;

namespace Isidore.MagicMirror.API.Controllers
{
    public class FacesController : NancyModule
    {
        private IFaceRecognitionService<byte[]> _faceService;

        public FacesController() : base("/faces")
        {
            this._faceService = new FisherFaceService();
            RegisterActions();
        }

        private void RegisterActions()
        {

            Get("/learn", _ =>
            {
                return $"Hello World, it's Nancy on .NET Core. {_}";
            });
        }
    }
}
