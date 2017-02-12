using System.Collections.Generic;

namespace Isidore.MagicMirror.ImageProcessing.FaceRecognition.Classifiers
{
   public interface IFaceClassifier<TImage>
    {
        IEnumerable<Area> DetectAllFaces(TImage image);

        Area RectangleDetectTheBiggestFace(TImage image);
    }
}