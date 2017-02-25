using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Isidore.MagicMirror.ImageProcessing.FaceRecognition.Classifiers;
using Isidore.MagicMirror.ImageProcessing.FaceRecognition.Services;
using Isidore.MagicMirror.ImageProcessing.Tests.FaceRecognitionTests;
using OpenCvSharp;
using Xunit;
using Microsoft.Extensions.FileProviders;
using System.Reflection;

namespace Isidore.MagicMirror.ImageProcessing.Tests
{
    public class FaceRecognitionFromBytesTests
    {
        IFaceClassifier<Mat> classifier;

        public FaceRecognitionFromBytesTests()
        {
            IFileProvider fileProvider = new EmbeddedFileProvider(typeof(TestClassifierTest).GetTypeInfo().Assembly);
            classifier = new HaarCascadeClassifier(fileProvider, "FaceClassifierTests.haarcascade_frontalface_default.xml");
        }
    
        
        [Theory]
        [InlineData("i292ua-fn.jpg", 292)]
        [InlineData("i293ua-fn.jpg", 293)]
        [InlineData("i294ua-mg.jpg", 294)]
        [InlineData("i295ua-mg.jpg", 295)]
        [InlineData("i296ua-mg.jpg", 296)]
        [InlineData("i297ua-mn.jpg", 297)]
        public async Task should_recognize_correctly_from_bytes(string imageSrc, int label)
        {
            var images = PhotoLoaderHelper.LoadPhotosByte("FaceRecognitionTests/TestPhotos", "i([0-9]{3}).*","ua-");
            var testedService = new FisherFaceByteProxy(classifier);
            await testedService.Learn(images);
            var users = images.Keys;

            
            var find = File.ReadAllBytes($"FaceRecognitionTests/TestPhotos/{imageSrc}");
            var result = await testedService.RecognizeAsync(find, users.ToList());

            Assert.Equal(label, result.RecognizedItem.Id);
        }
    }
}
