using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Isidore.MagicMirror.ImageProcessing.FaceRecognition.Classifiers;
using Isidore.MagicMirror.ImageProcessing.Helpers;
using OpenCvSharp;
using OpenCvSharp.Face;

namespace Isidore.MagicMirror.ImageProcessing.FaceRecognition
{
    public class FisherFaceService : IFaceRecognitionService<Mat>
    {
        private const double threshold = 90;
        private const double ConfidenceScaleBase = 50;
        private const int minFaceSize = 144;

        public FisherFaceService(IFaceClassifier<Mat> faceClasifier, string fileName = null)
        {

            if (String.IsNullOrWhiteSpace(fileName))
            {
                fileName = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            }

            trainingFile = fileName;
            classifier = faceClasifier;
        }

        private readonly string trainingFile;
        private readonly IFaceClassifier<Mat> classifier;


        public RecognitionResult<Person> Recognize(Mat image, IList<Person> users, string savedTrainingFile = null)
        {
            return this.RecognizeAsync(image, users, savedTrainingFile).Result;
        }

        public Task<RecognitionResult<Person>> RecognizeAsync(Mat image, IList<Person> users, string savedTrainingFile = null)
        {
            return new Task<RecognitionResult<Person>>(() =>
            {
                string usedFile;
                if (!String.IsNullOrWhiteSpace(savedTrainingFile))
                {
                    usedFile = savedTrainingFile;
                }
                else
                {
                    usedFile = trainingFile;
                }

                if (!File.Exists(usedFile))
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

                var result = new RecognitionResult<Person>();
                try
                {
                    using (var ffr = FaceRecognizer.CreateLBPHFaceRecognizer())
                    {
                        ffr.Load(usedFile);
                        var size = new Size(minFaceSize, minFaceSize);
                        var resizedFace = facePic.Scale(size);

                        prediction = ffr.Predict(resizedFace);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                var user = users.SingleOrDefault(x => x.Id == prediction);
                if (user != null)
                {
                    result.RecognizedItem = user;
                }
                else
                {
                    result.RecognizedItem = new Person
                    {
                        Id = prediction,
                        Name = $"Unknown {prediction}"
                    };
                }

                result.Area = faceRec;
                return result;
            });
        }

        public async Task Learn(IDictionary<Person, IEnumerable<Mat>> imagesWithLabels)
        {
            Action<LBPHFaceRecognizer, Mat[], int[]> action = (ffr, images, labels) =>
            {
                ffr.Train(images, labels);
            };

            await this.LearnTemplateMethod(imagesWithLabels, this.trainingFile, action);
        }

        public async Task LearnMore(IDictionary<Person, IEnumerable<Mat>> imagesWithLabels, string savedTrainingFile)
        {
            Action<LBPHFaceRecognizer, Mat[], int[]> action = (ffr, images, labels) =>
            {
                ffr.Load(savedTrainingFile);
                ffr.Update(images, labels);
            };

            await LearnTemplateMethod(imagesWithLabels, savedTrainingFile, action);
        }

        private async Task LearnTemplateMethod(
            IDictionary<Person, IEnumerable<Mat>> imagesWithLabels,
            string savedTrainingFile,
            Action<LBPHFaceRecognizer, Mat[], int[]> learnAction)
        {
            var trainingFaces = new LinkedList<Mat>();
            var labels = new LinkedList<int>();
            var normalSize = new OpenCvSharp.Size(minFaceSize, minFaceSize);

            foreach (var user in imagesWithLabels)
            {
                foreach (var photo in user.Value)
                {
                    var face = trainingFaces.AddLast(new Mat(GetFaceImage(photo).Resize(normalSize)));
                    labels.AddLast(user.Key.Id);
                }
            }

            try
            {
                using (var ffr = FaceRecognizer.CreateLBPHFaceRecognizer(threshold: threshold))
                {
                    await new Task(() => learnAction(ffr, trainingFaces.ToArray(), labels.ToArray()));
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