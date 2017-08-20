using System.Threading.Tasks;
using Isidore.MagicMirror.Infrastructure.Services;
using Isidore.MagicMirror.Users.Models;

namespace Isidore.MagicMirror.Users.Contract
{
    public interface IUserGroupService : IDataService<UserGroup>, IAsyncDataService<UserGroup>
    {
        Task<UserGroup> GetCurrentUserGroup();
    }
}
