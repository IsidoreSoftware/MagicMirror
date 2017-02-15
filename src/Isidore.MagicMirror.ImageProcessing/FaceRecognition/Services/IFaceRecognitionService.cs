using System.Collections.Generic;
using System.Threading.Tasks;
using Isidore.MagicMirror.ImageProcessing.FaceRecognition.Models;

namespace Isidore.MagicMirror.ImageProcessing.FaceRecognition.Services
{
    public interface IFaceRecognitionService<TImage>
    {
        Task<RecognitionResult<Person>> RecognizeAsync(TImage image, IList<Person> users, string savedTrainingFile = null);
        
        RecognitionResult<Person> Recognize(TImage image, IList<Person> users, string savedTrainingFile = null);

        Task Learn(IDictionary<Person, IEnumerable<TImage>> imagesWithLabels);

        Task LearnMore(IDictionary<Person, IEnumerable<TImage>> imagesWithLabels, string savedTrainingFile);
    }
}