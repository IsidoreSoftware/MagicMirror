using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;
using OpenCvSharp;
using Xunit;
using System;
using Isidore.MagicMirror.ImageProcessing.FaceRecognition.Classifiers;
using System.Threading.Tasks;
using Isidore.MagicMirror.ImageProcessing.FaceRecognition.Services;
using Isidore.MagicMirror.ImageProcessing.FaceRecognition.Models;
using Microsoft.Extensions.FileProviders;

namespace Isidore.MagicMirror.ImageProcessing.Tests.FaceRecognitionTests
{
    public class FaceRecognitionTests : IDisposable
    {
        IDictionary<Person, IEnumerable<Mat>> faceDatabase;
        IFileProvider fileProvider;

        public FaceRecognitionTests()
        {
            fileProvider = TestMocker
                   .MockFileProvider("FaceClassifierTests/haarcascade_frontalface_default.xml");
        }

        [Theory]
        [InlineData("i292ua-fn.jpg", 292)]
        [InlineData("i293ua-fn.jpg", 293)]
        [InlineData("i294ua-mg.jpg", 294)]
        [InlineData("i295ua-mg.jpg", 295)]
        [InlineData("i296ua-mg.jpg", 296)]
        [InlineData("i297ua-mn.jpg", 297)]
        public async Task when_given_the_same_face_should_recognize_correctly(string imageSrc, int label)
        {
            faceDatabase = PhotoLoaderHelper.LoadPhotos("FaceRecognitionTests/TestPhotos", "i([0-9]{3}).*");
            IFaceClassifier<Mat> classifier= new HaarCascadeClassifier(fileProvider);
            var identityRecognizer = new FisherFaceService(classifier);
            var users = faceDatabase.Keys;

            //When
            await identityRecognizer.Learn(faceDatabase);

            //Then
            var find = new Mat($"FaceRecognitionTests/TestPhotos/{imageSrc}", ImreadModes.GrayScale);
            var result = await identityRecognizer.RecognizeAsync(find, users.ToList());

            Assert.Equal(label, result.RecognizedItem.Id);
        }

        [Theory]
        [InlineData("i292ua-fn.jpg", 292)]
        [InlineData("i293ua-fn.jpg", 293)]
        [InlineData("i294ua-mg.jpg", 294)]
        [InlineData("i295ua-mg.jpg", 295)]
        [InlineData("i296ua-mg.jpg", 296)]
        [InlineData("i297ua-mn.jpg", 297)]
        public async Task when_given_the_similar_face_should_recognize_correctly(string imageSrc, int label)
        {
            faceDatabase = PhotoLoaderHelper.LoadPhotos("FaceRecognitionTests/TestPhotos", "i([0-9]{3}).*","ua-");
            IFaceClassifier<Mat> classifier = new HaarCascadeClassifier(fileProvider);
            var identityRecognizer = new FisherFaceService(classifier);
            var users = faceDatabase.Keys;

            //When
            await identityRecognizer.Learn(faceDatabase);

            //Then
            var find = new Mat($"FaceRecognitionTests/TestPhotos/{imageSrc}", ImreadModes.GrayScale);
            var result = await identityRecognizer.RecognizeAsync(find, users.ToList());

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