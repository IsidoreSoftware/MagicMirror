using Isidore.MagicMirror.ImageProcessing.FaceRecognition.Models;
using System;

namespace Isidore.MagicMirror.API.Services
{
    public class UserService
    {
        public Person GetPersonById(string id)
        {
            return new Person
            {
                Id = Int32.Parse(id),
                Name = $"Person no {id}"
            };
        }
    }
}
