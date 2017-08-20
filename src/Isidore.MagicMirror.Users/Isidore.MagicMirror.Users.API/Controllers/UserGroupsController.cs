using System.Linq;
using System.Threading.Tasks;
using Isidore.MagicMirror.Users.Contract;
using Nancy;

namespace Isidore.MagicMirror.Users.API.Controllers
{
    public class UserGroupsController : NancyModule
    {
        private readonly IUserGroupService _groupService;

        public UserGroupsController(IUserGroupService groupService) : base("groups")
        {
            _groupService = groupService;
            RegisterActions();
        }

        private void RegisterActions()
        {
            Get("", async (_, ctx) => await ListGroups());
        }

        private async Task<Response> ListGroups()
        {
            var result = (await _groupService.GetAllAsync()).ToList();
            if (result == null || !result.Any())
            {
                return Response.AsJson(result).WithStatusCode(204);
            }
            else
            {
                return Response.AsJson(result);
            }
        }
    }
}
