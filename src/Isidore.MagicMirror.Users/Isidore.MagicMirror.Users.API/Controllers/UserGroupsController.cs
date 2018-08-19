using System;
using System.Linq;
using System.Threading.Tasks;
using Isidore.MagicMirror.Users.Contract;
using Isidore.MagicMirror.Users.Models;
using Isidore.MagicMirror.WebService.Exceptions;
using Nancy;
using Nancy.ModelBinding;
using NLog;

namespace Isidore.MagicMirror.Users.API.Controllers
{
    public class UserGroupsController : NancyModule
    {
        private readonly IUserGroupService _groupService;
        private readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public UserGroupsController(IUserGroupService groupService) : base("groups")
        {
            _groupService = groupService;
            RegisterActions();
        }

        private void RegisterActions()
        {
            Get("", async (_, ctx) => await ListGroups());
            Delete("{id}", async (_, ctx) => await DeleteGroup(_["id"]));
            Post("", async (_, ctx) => await AddUserGroup(this.Bind<UserGroup>()));
        }

        private async Task<Response> ListGroups()
        {
            var result = (await _groupService.GetAllAsync()).ToList();
            if (result == null || !result.Any())
            {
                return HttpStatusCode.NoContent;
            }

            return Response.AsJson(result);
        }

        private async Task<Response> AddUserGroup(UserGroup userGroup)
        {
            try
            {
                await _groupService.InsertAsync(userGroup);
            }
            catch (Exception e)
            {
                Logger.Warn(e);
                return new NotFoundResponse();
            }

            return Nancy.Response.NoBody;
        }

        private async Task<Response> DeleteGroup(string groupId)
        {
            try
            {
                await _groupService.DeleteAsync(groupId);
            }
            catch (ElementNotFoundException e)
            {
                Logger.Warn(e);
                return new NotFoundResponse();
            }

            return Nancy.Response.NoBody;
        }
    }
}
