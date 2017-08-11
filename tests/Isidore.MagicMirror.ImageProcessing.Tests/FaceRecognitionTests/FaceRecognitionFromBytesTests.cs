using System;
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
using Isidore.MagicMirror.Users.Contract;
using FakeItEasy;
using Isidore.MagicMirror.Users.Models;

namespace Isidore.MagicMirror.ImageProcessing.Tests
{
    public class FaceRecognitionFromBytesTests
    {
        IFaceClassifier<Mat> classifier;

        public FaceRecognitionFromBytesTests()
        {
            var testAssembly = typeof(FaceRecognitionFromBytesTests).GetTypeInfo().Assembly;
            IFileProvider fileProvider = new EmbeddedFileProvider(testAssembly);
            classifier = new HaarCascadeClassifier(fileProvider, "FaceClassifierTests.haarcascade_frontalface_default.xml");
        }


        [Theory]
        [InlineData("i292ua-fn.jpg", "292")]
        [InlineData("i293ua-fn.jpg", "293")]
        [InlineData("i294ua-mg.jpg", "294")]
        [InlineData("i295ua-mg.jpg", "295")]
        [InlineData("i296ua-mg.jpg", "296")]
        [InlineData("i297ua-mn.jpg", "297")]
        public async Task should_recognize_correctly_from_bytes(string imageSrc, string label)
        {
            var path = PhotoLoaderHelper.GetLocalPath($"FaceRecognitionTests{Path.DirectorySeparatorChar}TestPhotos");

            var learningFile = Path.GetTempFileName();
            var images = PhotoLoaderHelper.LoadPhotosByte(path, "i([0-9]{3}).*", "ua-");
            var users = images.Keys;

            var userServiceMock = A.Fake<IUserService>();
            A.CallTo(() => userServiceMock.GetById(A<string>.That.IsEqualTo(label)))
                .Returns(new User() { Id = label });

            var testedService = new FisherFaceByteProxy(classifier, learningFile, userServiceMock);

            await testedService.LearnMore(images);

            var find = File.ReadAllBytes($"{path}{Path.DirectorySeparatorChar}{imageSrc}");
            var result = await testedService.RecognizeAsync(find);

            Assert.Equal(label, result.RecognizedItem.Id);
        }
    }
}
