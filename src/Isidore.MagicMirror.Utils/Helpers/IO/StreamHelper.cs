using System.IO;
using System.Threading.Tasks;

namespace Isidore.MagicMirror.Utils.Helpers.IO
{
    public static class StreamHelper
    {
        public static async Task<byte[]> ToByteArray(this Stream stream)
        {
            byte[] bytes = new byte[stream.Length + 10];
            int numBytesToRead = (int)stream.Length;
            int numBytesRead = 0;
            do
            {
                // Read may return anything from 0 to 10.
                int n = await stream.ReadAsync(bytes, numBytesRead, 10);
                numBytesRead += n;
                numBytesToRead -= n;
            } while (numBytesToRead > 0);

            return bytes;
        }
    }
}
