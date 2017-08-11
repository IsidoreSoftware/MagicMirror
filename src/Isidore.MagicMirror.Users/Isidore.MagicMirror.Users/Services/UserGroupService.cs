using System.Threading.Tasks;
using Isidore.MagicMirror.DAL.MongoDB;
using Isidore.MagicMirror.Users.Contract;
using Isidore.MagicMirror.Users.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Isidore.MagicMirror.Users.Services
{
    public class UserGroupService : MongoDataService<UserGroup>, IUserGroupService
    {
        private readonly string _groupId;
        private UserGroup _currentUserGroup;
        private static readonly object SyncRoot = new object();

        public UserGroupService(IMongoDatabase mongoDatabase, string groupId) : base(mongoDatabase,"userGroups")
        {
            _groupId = groupId;
        }

        protected override string EntityIdPropertyName => "Id";

        public async Task<UserGroup> GetCurrentUserGroup()
        {
            if (_currentUserGroup == null)
            {
                lock (SyncRoot)
                {
                    if (_currentUserGroup == null)
                        _currentUserGroup = GetById(_groupId);
                }
            }

            //TODO: remove
            if (_currentUserGroup == null)
            {
                _currentUserGroup = new UserGroup
                {
                    Id = _groupId,
                    GroupName = "TestGroup"
                };
            }

            return await Task.FromResult(_currentUserGroup);
        }
    }
}
