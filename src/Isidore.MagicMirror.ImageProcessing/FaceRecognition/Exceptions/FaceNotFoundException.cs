using System;

namespace Isidore.MagicMirror.ImageProcessing.FaceRecognition.Exceptions
{
    public class FaceNotFoundException : Exception
    {
        public FaceNotFoundException() : base("No faces was found in this picture")
        {
        }
    }
}