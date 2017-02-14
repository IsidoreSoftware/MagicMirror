using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;
using OpenCvSharp;
using Xunit;
using System;
using Isidore.MagicMirror.ImageProcessing.FaceRecognition;
using Isidore.MagicMirror.ImageProcessing.FaceRecognition.Classifiers;
using System.Threading.Tasks;

namespace Isidore.MagicMirror.ImageProcessing.Tests.FaceRecognitionTests
{
    public class FaceRecognitionTests : IDisposable
    {
        IDictionary<Person, IEnumerable<Mat>> faceDatabase;

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
            var classifier = new HaarCascadeClassifier("FaceClassifierTests");
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
            var classifier = new HaarCascadeClassifier("FaceClassifierTests");
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

    internal static class PhotoLoaderHelper
    {
        public static IDictionary<Person, IEnumerable<Mat>> LoadPhotos(string path, string classRegex, string exclusion = null)
        {
            var dictionary = new Dictionary<Person, IEnumerable<Mat>>();
            var files = Directory.GetFiles(path).Where(x => String.IsNullOrWhiteSpace(exclusion) || !x.Contains(exclusion)).Select(x => Path.GetFileName(x));

            foreach (var file in files)
            {
                var m = Regex.Match(file, classRegex);
                Person label = new Person()
                {
                    Name = m.Groups[1].Value,
                    Id = int.Parse(m.Groups[1].Value)
                };

                if (!dictionary.Keys.Any(x => x.Id == label.Id))
                    dictionary.Add(label, new List<Mat>());

                (dictionary.First(x => x.Key.Id == label.Id).Value as List<Mat>).Add(new Mat(Path.Combine(path, file), ImreadModes.GrayScale));
            }

            return dictionary;
        }
    }
}