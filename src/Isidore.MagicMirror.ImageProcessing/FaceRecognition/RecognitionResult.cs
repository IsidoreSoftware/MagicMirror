namespace Isidore.MagicMirror.ImageProcessing.FaceRecognition
{
    public class RecognitionResult<T>
    {
        public T RecognizedItem { get; set; }
        public double Confidence { get; set; }
        public Area Area { get; set; }
    }
}