using System.Linq;
using System.Threading.Tasks;
using Isidore.MagicMirror.DAL.MongoDB;
using Isidore.MagicMirror.Users.Contract;
using Isidore.MagicMirror.Users.Models;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using MongoDB.Driver;

namespace Isidore.MagicMirror.Users.Services
{
    public class UserGroupService : MongoDataService<UserGroup>, IUserGroupService
    {
        private readonly IFaceServiceClient _faceServiceClient;
        private Person[] _persons;
        private UserGroup _currentUserGroup;
        private static readonly object SyncRoot = new object();

        public UserGroupService(IMongoDatabase mongoDatabase, IFaceServiceClient faceServiceClient) : base(mongoDatabase,"userGroups")
        {
            _faceServiceClient = faceServiceClient;
        }

        protected override string EntityIdPropertyName => "Id";

        public async Task<UserGroup> GetCurrentUserGroup()
        {
            if (_currentUserGroup == null)
            {
                var group = (await _faceServiceClient.ListPersonGroupsAsync("", 1)).FirstOrDefault();
                lock (SyncRoot)
                {
                    if (_currentUserGroup == null)
                        _currentUserGroup = GetById(group.PersonGroupId);
                }

                //TODO: remove
                if (_currentUserGroup == null)
                {
                    _currentUserGroup = new UserGroup
                    {
                        Id = group.PersonGroupId,
                        GroupName = group.Name
                    };
                    await InsertAsync(_currentUserGroup);
                }
            }

            return await Task.FromResult(_currentUserGroup);
        }
    }
}
