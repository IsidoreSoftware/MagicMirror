using Isidore.MagicMirror.Infrastructure.Services;
using Isidore.MagicMirror.Users.Models;
using Isidore.MagicMirror.Users.Services;
using Xunit;

namespace Isidore.MagicMirror.Users.Tests
{
    public class UserServiceTests
    {
        public UserServiceTests()
        {
        }

        [Fact]
        public void can_create_user_service()
        {

            var userService = new UserService();
        }

        [Fact]
        public void user_service_should_implement_base_data_service()
        {
            Assert.IsAssignableFrom(typeof(IDataService<User>), new UserService());
        }

        [Fact]
        public void get_all_users_return_all()
        {
            var userService = new UserService();

            var users = userService.GetAll();

            Assert.Equal(5, users.Length);
        }
    }
}
