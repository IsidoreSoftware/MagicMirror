using Isidore.MagicMirror.Users.Models;
using Isidore.MagicMirror.DAL.MongoDB;
using MongoDB.Driver;
using Isidore.MagicMirror.Users.Contract;

namespace Isidore.MagicMirror.Users.Services
{
    public class UserService : MongoDataService<User>, IUserService
    {
        public UserService(IMongoDatabase database) : base(database, "users")
        {
        }

        protected override string EntityIdPropertyName => "UserGuid";
    }
}
