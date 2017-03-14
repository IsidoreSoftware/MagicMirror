using System.Threading.Tasks;

namespace Isidore.MagicMirror.ImageProcessing.ImageConversion
{
    public interface IImageConverter<T1,T2>
    {
        Task<T1> ConvertAsync(T2 inputImage);
        Task<T2> ConvertAsync(T1 inputImage);

        bool TryConvert(T1 inputImage, out T2 output);
        bool TryConvert(T2 inputImage, out T1 output);
    }
}
