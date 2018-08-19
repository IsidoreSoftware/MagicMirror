using Isidore.MagicMirror.DAL.MongoDB;

namespace Isidore.MagicMirror.Users.Models
{
    public class UserGroup : BaseMongoObject
    {
        public string GroupName { get; set; }
    }
}
