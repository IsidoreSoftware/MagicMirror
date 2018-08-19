using System.Collections.Generic;
using Microsoft.Extensions.FileProviders;
using OpenCvSharp;
using Xunit;
using System;
using Isidore.MagicMirror.ImageProcessing.FaceRecognition.Classifiers;
using System.Threading.Tasks;
using Isidore.MagicMirror.ImageProcessing.FaceRecognition.Services;
using Isidore.MagicMirror.Users.Models;
using System.Reflection;
using System.IO;
using Isidore.MagicMirror.Users.Contract;
using FakeItEasy;
using Microsoft.Extensions.Logging;

namespace Isidore.MagicMirror.ImageProcessing.Tests.FaceRecognitionTests
{
    public class FaceRecognitionTests : IDisposable
    {
        IDictionary<User, IEnumerable<Mat>> faceDatabase;
        readonly IFileProvider fileProvider;
        readonly string learningFile;
        private readonly ILogger<FisherFaceService> loggerMock;

        public FaceRecognitionTests()
        {
            fileProvider = new EmbeddedFileProvider(typeof(TestClassifierTest).GetTypeInfo().Assembly);
            learningFile = Path.GetTempFileName();
            loggerMock = A.Fake<ILogger<FisherFaceService>>();
        }

        [Theory]
        [InlineData("i292ua-fn.jpg", "292")]
        [InlineData("i293ua-fn.jpg", "293")]
        [InlineData("i294ua-mg.jpg", "294")]
        [InlineData("i295ua-mg.jpg", "295")]
        [InlineData("i296ua-mg.jpg", "296")]
        [InlineData("i297ua-mn.jpg", "297")]
        public async Task when_given_the_same_face_should_recognize_correctly(string imageSrc, string label)
        {
            var path = PhotoLoaderHelper.GetLocalPath($"FaceRecognitionTests{Path.DirectorySeparatorChar}TestPhotos");
            faceDatabase = PhotoLoaderHelper.LoadPhotos(path, "i([0-9]{3}).*");
            IFaceClassifier<Mat> classifier = new HaarCascadeClassifier(fileProvider, "FaceClassifierTests.haarcascade_frontalface_default.xml");
            var userServiceMock = A.Fake<IUserService>();
            A.CallTo(() => userServiceMock.GetById(A<string>.That.IsEqualTo(label)))
                .Returns(new User { Id = label });

            var identityRecognizer = new FisherFaceService(classifier, learningFile, userServiceMock,loggerMock);

            //When
            await identityRecognizer.LearnMore(faceDatabase);

            //Then
            var find = new Mat($"{path}{Path.DirectorySeparatorChar}{imageSrc}", ImreadModes.GrayScale);
            var result = await identityRecognizer.RecognizeAsync(find);

            Assert.Equal(label, result.RecognizedItem.Id);
        }

        [Theory]
        [InlineData("i292ua-fn.jpg", "292")]
        [InlineData("i293ua-fn.jpg", "293")]
        [InlineData("i294ua-mg.jpg", "294")]
        [InlineData("i295ua-mg.jpg", "295")]
        [InlineData("i296ua-mg.jpg", "296")]
        [InlineData("i297ua-mn.jpg", "297")]
        public async Task when_given_the_similar_face_should_recognize_correctly(string imageSrc, string label)
        {
            var path = PhotoLoaderHelper.GetLocalPath($"FaceRecognitionTests{Path.DirectorySeparatorChar}TestPhotos");
            faceDatabase = PhotoLoaderHelper.LoadPhotos(path, "i([0-9]{3}).*", "ua-");
            IFaceClassifier<Mat> classifier = new HaarCascadeClassifier(fileProvider, "FaceClassifierTests.haarcascade_frontalface_default.xml");

            var userServiceMock = A.Fake<IUserService>();
            A.CallTo(() => userServiceMock.GetById(A<string>.That.IsEqualTo(label)))
                .Returns(new User{ Id = label });

            var identityRecognizer = new FisherFaceService(classifier, learningFile, userServiceMock, loggerMock);

            //When
            await identityRecognizer.LearnMore(faceDatabase);

            //Then
            var find = new Mat($"{path}{Path.DirectorySeparatorChar}{imageSrc}", ImreadModes.GrayScale);
            var result = await identityRecognizer.RecognizeAsync(find);

            Assert.Equal(label, result.RecognizedItem.Id);
        }

        public void Dispose()
        {
            foreach (var el in faceDatabase)
            {
                foreach (var image in el.Value)
                {
                    image.Dispose();
                }
            }
        }
    }

}