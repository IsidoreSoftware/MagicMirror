namespace Isidore.MagicMirror.ImageProcessing.FaceRecognition.Models
{
    public class RecognitionResult<T>
    {
        public T RecognizedItem { get; set; }
        public double Distance { get; set; }
        public Area Area { get; set; }
    }
}