using Isidore.MagicMirror.Infrastructure.Services;
using Isidore.MagicMirror.Users.Models;
using Newtonsoft.Json;

namespace Isidore.MagicMirror.Users.Contract
{
    public struct UserFilter : IFilter<User>
    {
        public UserFilter(User patternUser)
        {
            QueryString = JsonConvert.SerializeObject(patternUser);
        }

        public string QueryString { get; private set; }
    }
}
