using System;

namespace Isidore.MagicMirror.ImageProcessing.FaceRecognition.Exceptions
{
    class RecognizedNotExistingUserException : Exception
    {
        public RecognizedNotExistingUserException(int userId):base("The user was recognized by classifier, but doesn't exist in user storage")
        {
            UserNo = userId;
        }

        public int UserNo { get; private set; }
    }
}
