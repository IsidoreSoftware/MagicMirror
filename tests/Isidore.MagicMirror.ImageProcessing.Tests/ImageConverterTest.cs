using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Isidore.MagicMirror.ImageProcessing.ImageConversion;
using Xunit;

namespace Isidore.MagicMirror.ImageProcessing.Tests
{
    public class ImageConverterTest : IDisposable
    {
        private readonly FilePathToBitmapConverter pathToBitmapConverter;
        private readonly string tempPath= Path.Combine(Path.GetTempPath(),"MagicMirror-tests");

        public ImageConverterTest()
        {
            if(!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);
            }

            pathToBitmapConverter = new FilePathToBitmapConverter(tempPath);
        }

        [Fact]
        public void null_path_throws_exception()
        {
            string path = null;
            Assert.ThrowsAsync(typeof(ArgumentNullException),() => pathToBitmapConverter.ConvertAsync(path));
        }

        [Fact]
        public void exception_on_not_existing_path()
        {
            string path = "C:/this_file_does_not_exist.jpg";
            Assert.ThrowsAsync(typeof(FileNotFoundException), () =>  pathToBitmapConverter.ConvertAsync(path));
        }

        [Fact]
        public async Task created_file_exists()
        {
            string imagePath;
            Bitmap bitmapToSave = new Bitmap(1,1); 
            imagePath = await pathToBitmapConverter.ConvertAsync(bitmapToSave);

            Assert.True(File.Exists(imagePath));
        }

        [Fact]
        public async Task created_file_has_jpg_extension()
        {
            string imagePath;
            Bitmap bitmapToSave = new Bitmap(1,1); 
            imagePath = await pathToBitmapConverter.ConvertAsync(bitmapToSave);

            Assert.EndsWith(".jpg",imagePath);
        }

        public void Dispose()
        {
            var files = Directory.GetFiles(tempPath);
            foreach(var file in files){
                File.Delete(file);                
            }
        }
    }
}
