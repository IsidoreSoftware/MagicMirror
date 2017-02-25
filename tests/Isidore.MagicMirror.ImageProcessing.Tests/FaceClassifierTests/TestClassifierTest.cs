using Xunit;
using System.Linq;
using Isidore.MagicMirror.ImageProcessing.FaceRecognition.Classifiers;
using Microsoft.Extensions.FileProviders;
using OpenCvSharp;
using System.Reflection;
using System.IO;
using Isidore.MagicMirror.ImageProcessing.Tests.FaceRecognitionTests;

namespace Isidore.MagicMirror.ImageProcessing.Tests
{
    public class TestClassifierTest
    {
        HaarCascadeClassifier classifier;
        string basePath;

        public TestClassifierTest()
        {
            IFileProvider fileProvider =new EmbeddedFileProvider(typeof(TestClassifierTest).GetTypeInfo().Assembly);
            classifier = new HaarCascadeClassifier(fileProvider, "FaceClassifierTests.haarcascade_frontalface_default.xml");

            basePath = PhotoLoaderHelper.GetLocalPath($"FaceClassifierTests{Path.DirectorySeparatorChar}");
        }

        [Fact]
        public void haar_classifier_can_get_face()
        {
            Mat image = new Mat($"{basePath}test_image.jpg");

            var face = classifier.RectangleDetectTheBiggestFace(image);

            Assert.NotNull(face);
        }

        [Fact]
        public void haar_classifier_can_get_correct_face()
        {
            Mat image = new Mat($"{basePath}test_image.jpg");

            var face = classifier.RectangleDetectTheBiggestFace(image);

            Assert.Equal(278, face.Height);
            Assert.Equal(278, face.Width);
            Assert.Equal(261, face.Top);
            Assert.Equal(611, face.Left);
        }

        [Fact]
        public void haar_classifier_can_get_more_than_one_face()
        {
            Mat image = new Mat($"{basePath}test_image2.jpg");

            var faces = classifier.DetectAllFaces(image);

            Assert.Equal(4, faces.Count());
        }

        [Fact]
        public void can_detect_the_biggest_face_among_many()
        {
            Mat image = new Mat($"{basePath}test_image2.jpg");

            var face = classifier.RectangleDetectTheBiggestFace(image);

            Assert.Equal(313, face.Top);
            Assert.Equal(75, face.Left);
            Assert.Equal(147, face.Width);
            Assert.Equal(147, face.Height);
        }
    }
}