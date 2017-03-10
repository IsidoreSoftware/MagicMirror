using FakeItEasy;
using Isidore.MagicMirror.Infrastructure.Services;
using Isidore.MagicMirror.Users.Contract;
using Isidore.MagicMirror.Users.Models;
using Isidore.MagicMirror.Users.Services;
using MongoDB.Driver;
using Xunit;

namespace Isidore.MagicMirror.Users.Tests
{
    public class UserServiceTests
    {
        private readonly IUserService _userService;
        private readonly IMongoCollection<User> _collectionMock;

        public UserServiceTests()
        {
            var client = new MongoClient();
            _collectionMock = A.Fake<IMongoCollection<User>>();
            var dbMock = A.Fake<IMongoDatabase>();
            A
                .CallTo(() => dbMock.GetCollection<User>(A<string>.Ignored, A<MongoCollectionSettings>.Ignored))
                .Returns(_collectionMock);

            _userService = new UserService(dbMock);
        }

        [Fact]
        public void user_service_should_implement_base_data_service()
        {
            Assert.IsAssignableFrom(
                typeof(IDataService<User>),
                new UserService(A.Fake<IMongoDatabase>()));
        }
    }
}
