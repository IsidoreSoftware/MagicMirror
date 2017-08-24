using System.Collections.Generic;
using Isidore.MagicMirror.Infrastructure.Services;
using Isidore.MagicMirror.Users.Contract;
using Isidore.MagicMirror.Users.Models;

namespace Isidore.MagicMirror.Users.Services
{
    public class CompositeUserService : CompositeDataService<User>, IUserService
    {
        public CompositeUserService(
            AzureUserService azureUserService,
            UserService dbUserService)
            : base(new HashSet<ISyncAndAsyncDataService<User>>(
                new List<ISyncAndAsyncDataService<User>> { dbUserService, azureUserService }))
        {
        }
    }
}
