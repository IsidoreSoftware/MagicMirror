using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Isidore.MagicMirror.ImageProcessing.FaceRecognition.Classifiers;
using Isidore.MagicMirror.ImageProcessing.FaceRecognition.Models;
using OpenCvSharp;
using Isidore.MagicMirror.Users.Models;
using Isidore.MagicMirror.Users.Contract;
using Microsoft.Extensions.Logging;
using System.IO;

namespace Isidore.MagicMirror.ImageProcessing.FaceRecognition.Services
{
    public class FisherFaceByteProxy : IFaceRecognitionService<Stream, User>
    {
        private readonly IFaceRecognitionService<Mat, User> _service;

        public FisherFaceByteProxy(IFaceClassifier<Mat> classifier, string fileName, IUserService userService, ILoggerFactory loggerFactory)
        {
            _service = new FisherFaceService(classifier, fileName, userService,
                loggerFactory.CreateLogger<FisherFaceService>());
        }

        public async Task LearnMore(IDictionary<User, IEnumerable<Stream>> imagesWithLabels)
        {
            var converted = ConvertImagesWithLabels(imagesWithLabels);
            await _service.LearnMore(converted);
        }

        public RecognitionResult<User> Recognize(Stream image)
        {
            Mat data = GetMatFromStream(image);

            return _service.Recognize(data);
        }

        public async Task<RecognitionResult<User>> RecognizeAsync(Stream image)
        {
            Mat data = GetMatFromStream(image);

            return await _service.RecognizeAsync(data);
        }

        private static IDictionary<User, IEnumerable<Mat>> ConvertImagesWithLabels(IDictionary<User, IEnumerable<Stream>> imagesWithLabels)
        {
            var converted = new Dictionary<User, IEnumerable<Mat>>(imagesWithLabels.Count);
            foreach (var person in imagesWithLabels)
            {
                var images = new List<Mat>(person.Value.Count());
                foreach (var image in person.Value)
                {
                    images.Add(GetMatFromStream(image));
                }
                converted.Add(person.Key, images);
            }

            return converted;
        }

        private static Mat GetMatFromStream(Stream image)
        {
            var result = new byte[image.Length];
            image.Read(result, 0, result.Length);
            return Mat.FromImageData(result, ImreadModes.Grayscale);
        }
    }
}