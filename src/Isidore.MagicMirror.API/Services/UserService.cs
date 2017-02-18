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

        public Person[] GetAllPersons()
        {
            return new Person[] {
                new Person
                {
                    Id = 123,
                    Name = $"Kuba Matjanowski"
                },
                new Person
                {
                    Id = 125,
                    Name = $"George Clooney"
                },
                 new Person
                {
                    Id = 126,
                    Name = $"Monika Zalewska"
                }};
        }
    }
}
