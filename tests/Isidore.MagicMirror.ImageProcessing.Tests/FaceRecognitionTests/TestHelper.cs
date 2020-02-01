using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using OpenCvSharp;
using Isidore.MagicMirror.Users.Models;
using System.Reflection;

namespace Isidore.MagicMirror.ImageProcessing.Tests.FaceRecognitionTests
{
    internal static class PhotoLoaderHelper
    {
        public static IDictionary<User, IEnumerable<Mat>> LoadPhotos(string path, string classRegex, string exclusion = null)
        {
            var dictionary = new Dictionary<User, IEnumerable<Mat>>();
            var files = Directory.GetFiles(path).Where(x => String.IsNullOrWhiteSpace(exclusion) || !x.Contains(exclusion)).Select(x => Path.GetFileName(x));

            foreach (var file in files)
            {
                var m = Regex.Match(file, classRegex);
                User label = new User()
                {
                    FirstName = m.Groups[1].Value,
                    Id = m.Groups[1].Value
                };

                if (dictionary.Keys.All(x => x.Id != label.Id))
                    dictionary.Add(label, new List<Mat>());
                var img = new Mat(Path.Combine(path, file), ImreadModes.Grayscale);
                (dictionary.First(x => x.Key.Id == label.Id).Value as List<Mat>).Add(img);
            }

            return dictionary;
        }

        public static IDictionary<User, IEnumerable<byte[]>> LoadPhotosByte(string path, string classRegex, string exclusion = null)
        {

            var dictionary = new Dictionary<User, IEnumerable<byte[]>>();
            var files = Directory.GetFiles(path).Where(x => String.IsNullOrWhiteSpace(exclusion) || !x.Contains(exclusion)).Select(x => Path.GetFileName(x));

            foreach (var file in files)
            {
                var m = Regex.Match(file, classRegex);
                User label = new User()
                {
                    FirstName = m.Groups[1].Value,
                    Id = m.Groups[1].Value
                };

                if (dictionary.Keys.All(x => x.Id != label.Id))
                    dictionary.Add(label, new List<byte[]>());


                (dictionary.First(x => x.Key.Id == label.Id).Value as List<byte[]>).Add(File.ReadAllBytes(Path.Combine(path, file)));
            }

            return dictionary;
        }

        public static string GetLocalPath(string relativePath)
        {
            var dllPath = typeof(PhotoLoaderHelper).GetTypeInfo().Assembly.CodeBase;
            var toCut = "file:///".Length;
            dllPath = dllPath.Substring(toCut, dllPath.Length - toCut);
            string path = Path.Combine(Path.GetDirectoryName(dllPath), relativePath.Replace("/", "\\")).ToString();
            return path;
        }
    }
}