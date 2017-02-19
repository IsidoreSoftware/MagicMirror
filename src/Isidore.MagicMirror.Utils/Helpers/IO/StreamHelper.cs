using System;
using System.IO;
using System.Threading.Tasks;

namespace Isidore.MagicMirror.Utils.Helpers.IO
{
    public static class StreamHelper
    {
        public  const int ByteCount = 4096;

        public static async Task<byte[]> ToByteArray(this Stream stream)
        {
            using (var memoryStream = new MemoryStream())
            {
                await stream.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}
