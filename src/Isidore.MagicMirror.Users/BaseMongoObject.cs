using MongoDB.Bson;

namespace Isidore.MagicMirror.Users.Models
{
    public class BaseMongoObject
    {
        public ObjectId _id { get; set; }
    }
}