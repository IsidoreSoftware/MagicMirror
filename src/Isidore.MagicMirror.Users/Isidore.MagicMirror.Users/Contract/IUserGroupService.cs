using System.Threading.Tasks;
using Isidore.MagicMirror.Infrastructure.Services;
using Isidore.MagicMirror.Users.Models;

namespace Isidore.MagicMirror.Users.Contract
{
    public interface IUserGroupService : ISyncAndAsyncDataService<UserGroup>
    {
        Task<UserGroup> GetCurrentUserGroup();
    }
}
