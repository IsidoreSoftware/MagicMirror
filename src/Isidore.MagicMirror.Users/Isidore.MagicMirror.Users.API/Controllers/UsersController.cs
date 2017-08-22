using System;
using System.Linq;
using System.Threading.Tasks;
using Isidore.MagicMirror.Infrastructure.Validation;
using Isidore.MagicMirror.Users.Contract;
using Isidore.MagicMirror.Users.Models;
using Isidore.MagicMirror.WebService.Exceptions;
using Nancy;
using Nancy.ModelBinding;
using NLog;

namespace Isidore.MagicMirror.Users.API.Controllers
{
    public class UsersController : NancyModule
    {
        private readonly IUserService _userService;
        private readonly IValidator _validator;
        private readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public UsersController(IUserService userService, ValidatorsFactory validatorsFactory) : base("/users")
        {
            _userService = userService;
            _validator = validatorsFactory.GetValidator<User>();
            RegisterActions();
        }

        private void RegisterActions()
        {
            Get("", async (_, ctx) => await ListUsers());
            Get("/{id}", async (_, ctx) => await GetUser(_["id"]));
            Post("", async (_, ctx) => await AddUser(this.Bind<User>())); 
            Delete("/{id}", async (_, ctx) => await RemoveUser(_["id"]));
        }

        private async Task<Response> ListUsers()
        {
            var result = (await _userService.GetAllAsync()).ToList();
            if(result == null || !result.Any())
            {
                return HttpStatusCode.NoContent;
            }
            return Response.AsJson(result);
        }

        private async Task<Response> GetUser(string id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
            {
                return HttpStatusCode.NotFound;
            }

            return Response.AsJson(user);
        }

        private async Task<Response> AddUser(User user)
        {
            var result = this._validator.Validate(user);

            if (!result.Result)
            {
                return Response.AsJson(result, HttpStatusCode.BadRequest);
            }

            user.RegistrationDate = DateTime.UtcNow;
            user.UserGuid = Guid.NewGuid().ToString("N");

            try
            {
                await _userService.InsertAsync(user);
                return Response.AsText(user.UserGuid).WithStatusCode(HttpStatusCode.Created);
            }
            catch (Exception e)
            {
                return Response.AsJson(e, HttpStatusCode.InternalServerError);
            }
        }

        private async Task<Response> RemoveUser(string id)
        {
            try
            {
                await _userService.DeleteAsync(id);
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