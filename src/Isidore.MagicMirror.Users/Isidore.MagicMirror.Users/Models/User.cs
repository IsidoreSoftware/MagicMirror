using System;
using Isidore.MagicMirror.DAL.MongoDB;

namespace Isidore.MagicMirror.Users.Models
{
    public class User : BaseMongoObject
    {
        public String UserGuid { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public DateTime RegistrationDate { get; set; }
    }
}
