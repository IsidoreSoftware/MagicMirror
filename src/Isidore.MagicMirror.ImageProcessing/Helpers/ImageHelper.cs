using Isidore.MagicMirror.ImageProcessing.FaceRecognition.Models;
using OpenCvSharp;

namespace Isidore.MagicMirror.ImageProcessing.Helpers
{
    public static class ImageHelper
    {
        public static Mat Crop(this Mat image, Area area)
        {
            return image.SubMat(area.Top, area.Top + area.Height, area.Left, area.Left + area.Width);
        }

        public static Mat Scale(this Mat image, Size size)
        {
            return image.Resize(size, 0, 0, InterpolationFlags.Lanczos4);
        }
    }
}