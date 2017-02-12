using System.Collections.Generic;
using System.Linq;
using Xunit;
using Isidore.MagicMirror.ImageProcessing.Helpers;
using OpenCvSharp;
using System.IO;
using Isidore.MagicMirror.ImageProcessing.FaceRecognition.Classifiers;

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
    }
}