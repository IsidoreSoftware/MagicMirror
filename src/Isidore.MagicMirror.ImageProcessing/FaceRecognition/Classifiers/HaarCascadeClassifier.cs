using System.Collections.Generic;
using System.Linq;
using OpenCvSharp;
using Isidore.MagicMirror.ImageProcessing.Helpers;
using System.IO;

namespace Isidore.MagicMirror.ImageProcessing.FaceRecognition.Classifiers
{
    public class HaarCascadeClassifier : IFaceClassifier<Mat>
    {
        private const string faceHaarCascade = "haarcascade_frontalface_default.xml";
        private readonly CascadeClassifier haarCascade;

        public HaarCascadeClassifier(string cascadeInfosBasePath)
        {
            var cascadeInfoFile = Path.Combine(cascadeInfosBasePath, faceHaarCascade);
            if(!File.Exists(cascadeInfoFile)){
                throw new FileNotFoundException("File with face definition can't be found");
            }

            haarCascade = new CascadeClassifier(cascadeInfoFile);
        }

        public IEnumerable<Area> DetectAllFaces(Mat image)
        {
            var faces = haarCascade.DetectMultiScale(image);

            return faces.Select(x => new Area
            {
                Top = x.Top,
                Left = x.Left,
                Width = x.Width,
                Height = x.Height
            });
        }

        public Area RectangleDetectTheBiggestFace(Mat image)
        {
            var faces = DetectAllFaces(image);

            if (faces != null && faces.Any())
            {
                return faces.MaxBy(x => x.Height + x.Width);
            }
            else
            {
                return null;
            }
        }
    }
}