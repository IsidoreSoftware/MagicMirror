using System;

namespace Isidore.MagicMirror.Users.Models
{
    public class User : BaseMongoObject
    {
        public int UserNo { get; set; }
        public String Name { get; set; }
    }
}
