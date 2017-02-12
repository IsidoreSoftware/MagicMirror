using Xunit;
using OpenCvSharp;
using System.IO;
using Isidore.MagicMirror.ImageProcessing.FaceRecognition.Classifiers;
using System.Linq;

namespace Isidore.MagicMirror.ImageProcessing.Tests
{
    public class TestClassifierTest
    {
        HaarCascadeClassifier classifier;

        public TestClassifierTest()
        {
            classifier = new HaarCascadeClassifier(
                Path.Combine(Directory.GetCurrentDirectory(), "FaceClassifierTests"));
        }

        [Fact]
        public void haar_classifier_can_get_face()
        {
            Mat image = new Mat("FaceClassifierTests/test_image.jpg");

            var face = classifier.RectangleDetectTheBiggestFace(image);

            Assert.NotNull(face);
        }

        [Fact]
        public void haar_classifier_can_get_correct_face()
        {
            Mat image = new Mat("FaceClassifierTests/test_image.jpg");

            var face = classifier.RectangleDetectTheBiggestFace(image);

            Assert.Equal(278, face.Height);
            Assert.Equal(278, face.Width);
            Assert.Equal(261, face.Top);
            Assert.Equal(611, face.Left);
        }

        [Fact]
        public void haar_classifier_can_get_more_than_one_face()
        {
            Mat image = new Mat("FaceClassifierTests/test_image2.jpg");

            var faces = classifier.DetectAllFaces(image);

            Assert.Equal(4, faces.Count());
        }

        [Fact]
        public void can_detect_the_biggest_face_among_many()
        {
            Mat image = new Mat("FaceClassifierTests/test_image2.jpg");

            var face = classifier.RectangleDetectTheBiggestFace(image);

            Assert.Equal(313, face.Top);
            Assert.Equal(75, face.Left);
            Assert.Equal(147, face.Width);
            Assert.Equal(147, face.Height);
        }
    }
}