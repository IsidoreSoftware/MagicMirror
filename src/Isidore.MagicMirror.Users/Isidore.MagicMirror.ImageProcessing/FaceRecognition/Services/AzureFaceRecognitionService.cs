using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Isidore.MagicMirror.ImageProcessing.FaceRecognition.Models;
using Isidore.MagicMirror.Users.Models;
using Microsoft.ProjectOxford.Face;

namespace Isidore.MagicMirror.ImageProcessing.FaceRecognition.Services
{
    public class AzureFaceRecognitionService : IFaceRecognitionService<byte[],User>
    {
        private readonly IFaceServiceClient _faceServiceClient;

        public AzureFaceRecognitionService(IFaceServiceClient faceServiceClient)
        {
            _faceServiceClient = faceServiceClient;
        }

        public Task<RecognitionResult<User>> RecognizeAsync(byte[] image)
        {
            throw new NotImplementedException();
        }

        public RecognitionResult<User> Recognize(byte[] image)
        {
            throw new NotImplementedException();
        }

        public Task LearnMore(IDictionary<User, IEnumerable<byte[]>> imagesWithLabels)
        {
            throw new NotImplementedException();
        }
    }
}
