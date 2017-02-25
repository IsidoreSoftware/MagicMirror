using System.Collections.Generic;
using System.Threading.Tasks;
using Isidore.MagicMirror.ImageProcessing.FaceRecognition.Models;

namespace Isidore.MagicMirror.ImageProcessing.FaceRecognition.Services
{
    public interface IFaceRecognitionService<TImage,TResult>
    {
        Task<RecognitionResult<TResult>> RecognizeAsync(TImage image, IList<TResult> users);
        
        RecognitionResult<TResult> Recognize(TImage image, IList<TResult> users);

        Task LearnMore(IDictionary<TResult, IEnumerable<TImage>> imagesWithLabels);
    }
}