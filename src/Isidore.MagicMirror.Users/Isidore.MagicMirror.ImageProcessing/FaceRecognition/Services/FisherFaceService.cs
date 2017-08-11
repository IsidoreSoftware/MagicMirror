using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Isidore.MagicMirror.ImageProcessing.FaceRecognition.Classifiers;
using Isidore.MagicMirror.ImageProcessing.Helpers;
using Isidore.MagicMirror.ImageProcessing.FaceRecognition.Models;
using OpenCvSharp;
using OpenCvSharp.Face;
using Isidore.MagicMirror.Users.Models;
using Isidore.MagicMirror.Users.Contract;
using Isidore.MagicMirror.ImageProcessing.FaceRecognition.Exceptions;

namespace Isidore.MagicMirror.ImageProcessing.FaceRecognition.Services
{
    public class FisherFaceService : IFaceRecognitionService<Mat, User>
    {
        private const double threshold = 90;
        private const double ConfidenceScaleBase = 50;
        private const int minFaceSize = 144;

        public FisherFaceService(IFaceClassifier<Mat> faceClasifier, string fileName, IUserService userService)
        {

            if (String.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException("Learning filename is not specified");
            }
            if (!File.Exists(fileName))
            {
                try
                {
                    File.Create(fileName);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Learning file doesn't exists and cannot be created: {ex.Message}", ex);
                }
            }

            trainingFile = fileName;
            classifier = faceClasifier;
            _userService = userService;
        }

        private readonly string trainingFile;
        private readonly IFaceClassifier<Mat> classifier;
        private readonly IUserService _userService;

        public RecognitionResult<User> Recognize(Mat image)
        {
            return this.RecognizeAsync(image).Result;
        }

        public async Task<RecognitionResult<User>> RecognizeAsync(Mat image)
        {
            return await Task.Factory.StartNew(() =>
            {
                if (!File.Exists(trainingFile))
                {
                    throw new FileNotFoundException();
                }

                var faceRec = classifier.RectangleDetectTheBiggestFace(image);
                Mat facePic = null;
                if (faceRec == null)
                {
                    return null;
                }
                else
                {
                    facePic = image.Crop(faceRec);
                }
                int prediction;
                double confidence;

                var result = new RecognitionResult<User>();
                try
                {
                    using (var ffr = FaceRecognizer.CreateLBPHFaceRecognizer())
                    {
                        ffr.Load(trainingFile);
                        var size = new Size(minFaceSize, minFaceSize);
                        var resizedFace = facePic.Scale(size);

                        ffr.Predict(resizedFace, out prediction, out confidence);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                var user = _userService.GetById(prediction.ToString());
                if (user != null)
                {
                    result.RecognizedItem = user;
                    result.Area = faceRec;
                    result.Distance = confidence;
                    return result;
                }
                else
                {
                    throw new RecognizedNotExistingUserException(prediction);
                }
            });
        }

        public async Task LearnMore(IDictionary<User, IEnumerable<Mat>> imagesWithLabels)
        {
            Action<LBPHFaceRecognizer, Mat[], string[]> action = (ffr, images, labels) =>
            {
                var fi = new FileInfo(trainingFile);
                if (fi.Length > 0)
                {
                    ffr.Load(trainingFile);
                    ffr.Update(images, labels.Select(int.Parse));
                }
                else
                {
                    ffr.Train(images, labels.Select(int.Parse));
                }
            };

            await LearnTemplateMethod(imagesWithLabels, trainingFile, action);
        }

        private async Task LearnTemplateMethod(
            IDictionary<User, IEnumerable<Mat>> imagesWithLabels,
            string savedTrainingFile,
            Action<LBPHFaceRecognizer, Mat[], string[]> learnAction)
        {
            var trainingFaces = new LinkedList<Mat>();
            var labels = new LinkedList<string>();
            var normalSize = new Size(minFaceSize, minFaceSize);

            foreach (var user in imagesWithLabels)
            {
                foreach (var photo in user.Value)
                {
                    var faceImage = GetFaceImage(photo).Resize(normalSize);
                    var face = trainingFaces.AddLast(faceImage);
                    labels.AddLast(user.Key.Id);
                }
            }

            try
            {
                using (var ffr = FaceRecognizer.CreateLBPHFaceRecognizer(threshold: threshold))
                {
                    await Task.Factory.StartNew(() => learnAction(ffr, trainingFaces.ToArray(), labels.ToArray()));
                    ffr.Save(savedTrainingFile);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private Mat GetFaceImage(Mat image)
        {
            var face = classifier.RectangleDetectTheBiggestFace(image);
            if (face == null)
                return null;

            var facePic = image.Crop(face);
            return facePic;
        }
    }
}