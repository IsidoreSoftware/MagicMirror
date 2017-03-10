using Isidore.MagicMirror.ImageProcessing.FaceRecognition.Models;
using Isidore.MagicMirror.ImageProcessing.Helpers;
using Isidore.MagicMirror.ImageProcessing.Tests.FaceRecognitionTests;
using OpenCvSharp;
using System.IO;
using Xunit;

namespace Isidore.MagicMirror.ImageProcessing.Tests
{
    public class ImageHelperTest
    {
        [Fact]
        public void cropping_image_should_return_image_with_required_size()
        {
            Mat image = new Mat(new Size(200, 200), MatType.CV_16S);

            var cropArea = new Area
            {
                Top = 0,
                Left = 0,
                Height = 20,
                Width = 30
            };

            Mat cropped = image.Crop(cropArea);

            Assert.Equal(20, cropped.Rows);
            Assert.Equal(30, cropped.Cols);
        }

        [Fact]
        public void cropped_image_should_have_the_same_pixels()
        {

            string basePath = PhotoLoaderHelper.GetLocalPath($"FaceClassifierTests{Path.DirectorySeparatorChar}");
            Mat image = new Mat($"{basePath}test_image.jpg");

            var cropArea = new Area
            {
                Top = 200,
                Left = 200,
                Height = 20,
                Width = 30
            };

            Mat cropped = image.Crop(cropArea);

            var oryg = image.At<Vec3b>(210, 210);
            var crop = cropped.At<Vec3b>(10, 10);

            Assert.Equal(oryg.Item0, crop.Item0);
            Assert.Equal(oryg.Item1, crop.Item1);
            Assert.Equal(oryg.Item2, crop.Item2);
        }

        [Fact]
        public void scaling_image_should_return_image_with_required_size()
        {
            Mat image = new Mat(new Size(200, 200), MatType.CV_16S);

            var cropArea = new Size
            {
                Height = 20,
                Width = 30
            };

            Mat scaled = image.Scale(cropArea);

            Assert.Equal(20, scaled.Rows);
            Assert.Equal(30, scaled.Cols);
        }
    }
}
