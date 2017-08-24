using System.Collections.Generic;
using System.Threading.Tasks;
using Isidore.MagicMirror.Infrastructure.Services;
using Isidore.MagicMirror.Users.Contract;
using Isidore.MagicMirror.Users.Models;

namespace Isidore.MagicMirror.Users.Services
{
    public class CompositeUserGroupService : CompositeDataService<UserGroup>, IUserGroupService
    {
        public CompositeUserGroupService(
            AzureUserGroupService azureService,
            UserGroupService service) :
            base(new HashSet<ISyncAndAsyncDataService<UserGroup>>(
                new List<ISyncAndAsyncDataService<UserGroup>> { service, azureService }))
        {
            
        }

        public async Task<UserGroup> GetCurrentUserGroup()
        {
            foreach (var dataService in Services)
            {
                var service = dataService as IUserGroupService;
                var user = await service.GetCurrentUserGroup();
                if (user != null)
                    return user;
            }

            return null;
        }
    }
}
