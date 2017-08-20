using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Isidore.MagicMirror.Infrastructure.Validation;
using Isidore.MagicMirror.Users.Contract;
using Isidore.MagicMirror.Users.Models;
using Nancy;
using Nancy.ModelBinding;
using Newtonsoft.Json;

namespace Isidore.MagicMirror.Users.API.Controllers
{
    public class UsersController : NancyModule
    {
        private readonly IUserService _userService;
        private readonly IValidator validator;

        public UsersController(IUserService userService, ValidatorsFactory validatorsFactory) : base("/users")
        {
            _userService = userService;
            validator = validatorsFactory.GetValidator<User>();
            RegisterActions();
        }

        private void RegisterActions()
        {
            Get("", async (_, ctx) => await ListUsers());
            Get("/{id}", async (_, ctx) => await GetUser(_["id"]));
            Post("", async (_, ctx) => await AddUser(this.Bind<User>()));
        }

        private async Task<Response> ListUsers()
        {
            var result = await _userService.GetAllAsync();
            if(result == null || !result.Any())
            {
                return Response.AsJson(result).WithStatusCode(204);
            }
            else
            {
                return Response.AsJson(result);
            }
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
            var result = this.validator.Validate(user);

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
    }
}