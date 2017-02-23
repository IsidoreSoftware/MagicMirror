using Isidore.MagicMirror.Users.Services;
using System.Threading.Tasks;
using Xunit;

namespace Isidore.MagicMirror.Users.Tests
{
    public class UserServiceTests
    {
        public UserServiceTests()
        {
        }

        [Fact]
        public async Task can_create_user_service()
        {
            var userService = new UserService();
        }
    }
}
