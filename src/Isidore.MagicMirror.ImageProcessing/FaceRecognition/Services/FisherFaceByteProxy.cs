using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Isidore.MagicMirror.ImageProcessing.FaceRecognition.Classifiers;
using Isidore.MagicMirror.ImageProcessing.FaceRecognition.Models;
using OpenCvSharp;
using Isidore.MagicMirror.Users.Models;

namespace Isidore.MagicMirror.ImageProcessing.FaceRecognition.Services
{
    public class FisherFaceByteProxy : IFaceRecognitionService<byte[],User>
    {
        private IFaceRecognitionService<Mat,User> _service;

        public FisherFaceByteProxy(IFaceClassifier<Mat> classifier, string fileName)
        {
            _service = new FisherFaceService(classifier,fileName);
        }

        public async Task LearnMore(IDictionary<User, IEnumerable<byte[]>> imagesWithLabels)
        {
            var converted = ConvertImagesWithLabels(imagesWithLabels);
            await this._service.LearnMore(converted);
        }

        public RecognitionResult<User> Recognize(byte[] image, IList<User> users)
        {
            Mat data = GetMatFromBytes(image);

            return _service.Recognize(data, users);
        }

        public async Task<RecognitionResult<User>> RecognizeAsync(byte[] image, IList<User> users)
        {
            Mat data = GetMatFromBytes(image);

            return await _service.RecognizeAsync(data, users);
        }

        private static IDictionary<User, IEnumerable<Mat>> ConvertImagesWithLabels(IDictionary<User, IEnumerable<byte[]>> imagesWithLabels)
        {
            var converted =  new Dictionary<User, IEnumerable<Mat>>(imagesWithLabels.Count);
            foreach (var person in imagesWithLabels)
            {
                var images = new List<Mat>(person.Value.Count());
                foreach (var image in person.Value)
                {
                    images.Add(GetMatFromBytes(image));
                }
                converted.Add(person.Key, images);
            }

            return converted;
        }

        private static Mat GetMatFromBytes(byte[] image)
        {
            return Mat.FromImageData(image, ImreadModes.GrayScale);
        }
    }
}