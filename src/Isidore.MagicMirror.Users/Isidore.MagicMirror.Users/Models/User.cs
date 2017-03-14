using System;

namespace Isidore.MagicMirror.Users.Models
{
    public class User : BaseMongoObject
    {
        public int UserNo { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public DateTime RegistrationDate { get; set; }
    }
}
