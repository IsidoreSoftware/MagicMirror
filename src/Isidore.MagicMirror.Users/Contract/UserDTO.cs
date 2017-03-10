using System;

namespace Isidore.MagicMirror.Users.Contract
{
    public struct UserDTO
    {
        public int UserNo { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public DateTime RegistrationDate { get; set; }
    }
}
