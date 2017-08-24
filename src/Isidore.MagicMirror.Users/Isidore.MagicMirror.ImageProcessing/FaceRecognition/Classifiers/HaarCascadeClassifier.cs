using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.FileProviders;
using OpenCvSharp;
using Isidore.MagicMirror.ImageProcessing.Helpers;
using Isidore.MagicMirror.ImageProcessing.FaceRecognition.Models;
using System.IO;
using System;

namespace Isidore.MagicMirror.ImageProcessing.FaceRecognition.Classifiers
{
    public class HaarCascadeClassifier : IFaceClassifier<Mat>
    {
        private const string defaultFaceHaarCascade = "haarcascade_frontalface_default.xml";
        private readonly CascadeClassifier haarCascade;

        public HaarCascadeClassifier(IFileProvider fileProvider, string cascadeFileName = null)
        {
            var cascadeFileInfo = fileProvider.GetFileInfo(cascadeFileName ?? defaultFaceHaarCascade);
            if (!cascadeFileInfo.Exists)
            {
                throw new FileNotFoundException("File with face definition can't be found");
            }

            string fullFileName = cascadeFileInfo.PhysicalPath;

            if (string.IsNullOrWhiteSpace(fullFileName))
            {
                fullFileName = Path.GetTempFileName();
                using (var outstream = File.OpenWrite(fullFileName))
                {
                    cascadeFileInfo.CreateReadStream().CopyTo(outstream);
                }
            }

            haarCascade = new CascadeClassifier(fullFileName);
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