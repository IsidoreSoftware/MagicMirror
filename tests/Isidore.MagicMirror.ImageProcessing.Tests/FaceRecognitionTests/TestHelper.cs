using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Isidore.MagicMirror.ImageProcessing.FaceRecognition.Models;
using OpenCvSharp;

namespace Isidore.MagicMirror.ImageProcessing.Tests.FaceRecognitionTests
{
    internal static class PhotoLoaderHelper
    {
        public static IDictionary<Person, IEnumerable<Mat>> LoadPhotos(string path, string classRegex, string exclusion = null)
        {
            var dictionary = new Dictionary<Person, IEnumerable<Mat>>();
            var files = Directory.GetFiles(path).Where(x => String.IsNullOrWhiteSpace(exclusion) || !x.Contains(exclusion)).Select(x => Path.GetFileName(x));

            foreach (var file in files)
            {
                var m = Regex.Match(file, classRegex);
                Person label = new Person()
                {
                    Name = m.Groups[1].Value,
                    Id = int.Parse(m.Groups[1].Value)
                };

                if (!dictionary.Keys.Any(x => x.Id == label.Id))
                    dictionary.Add(label, new List<Mat>());
                var img = new Mat(Path.Combine(path, file), ImreadModes.GrayScale);
                (dictionary.First(x => x.Key.Id == label.Id).Value as List<Mat>).Add(img);
            }

            return dictionary;
        }

        public static IDictionary<Person, IEnumerable<byte[]>> LoadPhotosByte(string path, string classRegex, string exclusion = null)
        {
            var dictionary = new Dictionary<Person, IEnumerable<byte[]>>();
            var files = Directory.GetFiles(path).Where(x => String.IsNullOrWhiteSpace(exclusion) || !x.Contains(exclusion)).Select(x => Path.GetFileName(x));

            foreach (var file in files)
            {
                var m = Regex.Match(file, classRegex);
                Person label = new Person()
                {
                    Name = m.Groups[1].Value,
                    Id = int.Parse(m.Groups[1].Value)
                };

                if (!dictionary.Keys.Any(x => x.Id == label.Id))
                    dictionary.Add(label, new List<byte[]>());


                (dictionary.First(x => x.Key.Id == label.Id).Value as List<byte[]>).Add(File.ReadAllBytes(Path.Combine(path, file)));
            }

            return dictionary;
        }
    }
}