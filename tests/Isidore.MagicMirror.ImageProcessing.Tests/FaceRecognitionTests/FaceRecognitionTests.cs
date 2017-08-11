using System.Collections.Generic;
using System.Linq;
using OpenCvSharp;
using Xunit;
using System;
using Isidore.MagicMirror.ImageProcessing.FaceRecognition.Classifiers;
using System.Threading.Tasks;
using Isidore.MagicMirror.ImageProcessing.FaceRecognition.Services;
using Microsoft.Extensions.FileProviders;
using Isidore.MagicMirror.Users.Models;
using System.Reflection;
using System.IO;
using Isidore.MagicMirror.Users.Contract;
using FakeItEasy;

namespace Isidore.MagicMirror.ImageProcessing.Tests.FaceRecognitionTests
{
    public class FaceRecognitionTests : IDisposable
    {
        IDictionary<User, IEnumerable<Mat>> faceDatabase;
        IFileProvider fileProvider;
        string learningFile;

        public FaceRecognitionTests()
        {
            fileProvider = new EmbeddedFileProvider(typeof(TestClassifierTest).GetTypeInfo().Assembly);
            learningFile = Path.GetTempFileName();
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
            var users = faceDatabase.Keys;
            var userServiceMock = A.Fake<IUserService>();
            A.CallTo(() => userServiceMock.GetById(A<string>.That.IsEqualTo(label)))
                .Returns(new User() { Id = label });

            var identityRecognizer = new FisherFaceService(classifier, learningFile, userServiceMock);

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

            var users = faceDatabase.Keys;
            var userServiceMock = A.Fake<IUserService>();
            A.CallTo(() => userServiceMock.GetById(A<string>.That.IsEqualTo(label.ToString())))
                .Returns(new User() { Id = label });

            var identityRecognizer = new FisherFaceService(classifier, learningFile, userServiceMock);

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