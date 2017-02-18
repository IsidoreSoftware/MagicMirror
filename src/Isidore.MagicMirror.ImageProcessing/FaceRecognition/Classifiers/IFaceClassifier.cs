using System.Collections.Generic;
using Isidore.MagicMirror.ImageProcessing.FaceRecognition.Models;

namespace Isidore.MagicMirror.ImageProcessing.FaceRecognition.Classifiers
{
    public interface IFaceClassifier<TImage>
    {
        IEnumerable<Area> DetectAllFaces(TImage image);

        Area RectangleDetectTheBiggestFace(TImage image);
    }
}