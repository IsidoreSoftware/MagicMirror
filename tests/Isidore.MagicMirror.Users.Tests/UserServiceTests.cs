using FakeItEasy;
using Isidore.MagicMirror.Infrastructure.Paging;
using Isidore.MagicMirror.Infrastructure.Services;
using Isidore.MagicMirror.Users.Contract;
using Isidore.MagicMirror.Users.Models;
using Isidore.MagicMirror.Users.Services;
using MongoDB.Driver;
using System.Linq;
using Xunit;

namespace Isidore.MagicMirror.Users.Tests
{
    public class UserServiceTests
    {
        readonly IUserService _userService;

        public UserServiceTests()
        {
            var client = new MongoClient();

            _userService = new UserService(client.GetDatabase("magic-mirror"));
        }

        [Fact]
        public void user_service_should_implement_base_data_service()
        {
            Assert.IsAssignableFrom(
                typeof(IDataService<User>),
                new UserService(A.Fake<IMongoDatabase>()));
        }

        [Fact]
        public void get_all_users_return_all()
        {
            var users = _userService.GetAll().ToList();
            Assert.Equal(5, users.Count);
        }

        [Fact]
        public void get_all_users_return_all_in_pages()
        {
            var pr = new PageReqest();
            pr.PageNumber = 2;
            pr.PageSize = 1;
            var usersPage = _userService.GetAll(pr);

            Assert.Equal(5, usersPage.TotalElementCount);
            Assert.Equal(1, usersPage.PageNumber);
            Assert.Equal(1, usersPage.PageSize);
        }

        [Fact]
        public void get_last_plus_one_page()
        {
            var pr = new PageReqest();
            pr.PageNumber = 6;
            pr.PageSize = 1;
            var usersPage = _userService.GetAll(pr);

            Assert.Equal(5, usersPage.TotalElementCount);
            Assert.Equal(5, usersPage.PageNumber);
            Assert.Equal(1, usersPage.PageSize);
            Assert.Equal(6, usersPage.RequestedPageSize);
        }
    }
}
