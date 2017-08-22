using System.Linq;
using System.Threading.Tasks;
using Isidore.MagicMirror.DAL.MongoDB;
using Isidore.MagicMirror.Users.Contract;
using Isidore.MagicMirror.Users.Models;
using Microsoft.ProjectOxford.Face;
using MongoDB.Driver;

namespace Isidore.MagicMirror.Users.Services
{
    public class UserGroupService : MongoDataService<UserGroup>, IUserGroupService
    {
        private UserGroup _currentUserGroup;
        private static readonly object SyncRoot = new object();

        public UserGroupService(IMongoDatabase mongoDatabase) : base(mongoDatabase,"userGroups")
        {
        }

        protected override string EntityIdPropertyName => "Id";

        public async Task<UserGroup> GetCurrentUserGroup()
        {
            if (_currentUserGroup == null)
            {
                var group  = (await GetAllAsync()).FirstOrDefault();
                lock (SyncRoot)
                {
                    if (_currentUserGroup == null)
                        _currentUserGroup = group;
                }
            }

            return await Task.FromResult(_currentUserGroup);
        }
    }
}
