using Isidore.MagicMirror.DAL.MongoDB;
using Isidore.MagicMirror.Users.Models;
using System;

namespace Isidore.MagicMirror.Users.Contract
{
    public class UserFilter : MongoDbFilter<User>
    {
        public int? UserNo { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public DateTime? RegistrationDate { get; set; }
    }
}
