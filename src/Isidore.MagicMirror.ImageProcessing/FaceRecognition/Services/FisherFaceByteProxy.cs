using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Isidore.MagicMirror.ImageProcessing.FaceRecognition.Classifiers;
using Isidore.MagicMirror.ImageProcessing.FaceRecognition.Models;
using OpenCvSharp;
using Isidore.MagicMirror.Users.Models;

namespace Isidore.MagicMirror.ImageProcessing.FaceRecognition.Services
{
    public class FisherFaceByteProxy : IFaceRecognitionService<byte[],Person>
    {
        private IFaceRecognitionService<Mat,Person> _service;

        public FisherFaceByteProxy(IFaceClassifier<Mat> classifier, string fileName = null)
        {
            _service = new FisherFaceService(classifier,fileName);
        }

        public async Task Learn(IDictionary<Person, IEnumerable<byte[]>> imagesWithLabels)
        {
            var converted = ConvertImagesWithLabels(imagesWithLabels);
            

            await this._service.Learn(converted);
        }

        public async Task LearnMore(IDictionary<Person, IEnumerable<byte[]>> imagesWithLabels, string savedTrainingFile)
        {
            var converted = ConvertImagesWithLabels(imagesWithLabels);
            await this._service.LearnMore(converted,savedTrainingFile);
        }

        public RecognitionResult<Person> Recognize(byte[] image, IList<Person> users, string savedTrainingFile = null)
        {
            Mat data = GetMatFromBytes(image);

            return _service.Recognize(data, users, savedTrainingFile);
        }

        public async Task<RecognitionResult<Person>> RecognizeAsync(byte[] image, IList<Person> users, string savedTrainingFile = null)
        {
            Mat data = GetMatFromBytes(image);

            return await _service.RecognizeAsync(data, users, savedTrainingFile);
        }

        private static IDictionary<Person, IEnumerable<Mat>> ConvertImagesWithLabels(IDictionary<Person, IEnumerable<byte[]>> imagesWithLabels)
        {
            var converted =  new Dictionary<Person, IEnumerable<Mat>>(imagesWithLabels.Count);
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

            //return OpenCvSharp.Cv2.ImDecode(image, ImreadModes.GrayScale);
        }
    }
}