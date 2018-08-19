using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace Isidore.MagicMirror.ImageProcessing.ImageConversion
{
    public class FilePathToBitmapConverter : IImageConverter<string, Bitmap>
    {
        private readonly string resultFolderPath;
        public FilePathToBitmapConverter(string resultFolderPath)
        {
            this.resultFolderPath = resultFolderPath;
        }

        public async Task<Bitmap> ConvertAsync(string inputImage)
        {
            if (String.IsNullOrEmpty(inputImage))
            {
                throw new ArgumentNullException(nameof(inputImage));
            }
            if (!File.Exists(inputImage))
            {
                throw new FileNotFoundException("This file doesn't exist");
            }

            Task<Bitmap> getImage = new Task<Bitmap>(()=>new Bitmap(File.Open(inputImage, FileMode.Open)));
            return await getImage;
        }

        public async Task<string> ConvertAsync(Bitmap inputImage)
        {
            var fileName = $"{Path.GetRandomFileName()}.jpg";
            var fullFilePath  = Path.Combine(resultFolderPath,fileName);
            using(var stream = File.OpenWrite(fullFilePath))
            {
                await Task.Run(()=> inputImage.Save(stream, ImageFormat.Jpeg));
            }

            return fullFilePath;
        }

        public bool TryConvert(Bitmap inputImage, out string output)
        {
            try
            {
                output = ConvertAsync(inputImage).Result;
                return true;
            }
            catch (Exception)
            {
                output = null;
                return false;
            }
        }

        public bool TryConvert(string inputImage, out Bitmap output)
        {
            try
            {
                output = this.ConvertAsync(inputImage).Result;
                return true;
            }
            catch (Exception)
            {
                output = null;
                return false;
            }
        }
    }
}