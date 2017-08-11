using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Isidore.MagicMirror.DAL.MongoDB
{
    public class BaseMongoObject
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
    }
}