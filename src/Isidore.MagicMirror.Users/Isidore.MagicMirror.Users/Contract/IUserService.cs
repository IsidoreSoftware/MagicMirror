using Isidore.MagicMirror.Infrastructure.Services;
using Isidore.MagicMirror.Users.Models;

namespace Isidore.MagicMirror.Users.Contract
{
    public interface IUserService : ISyncAndAsyncDataService<User>
    {
    }
}
