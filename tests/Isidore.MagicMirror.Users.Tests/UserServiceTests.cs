using FakeItEasy;
using Isidore.MagicMirror.Infrastructure.Services;
using Isidore.MagicMirror.Users.Contract;
using Isidore.MagicMirror.Users.Models;
using Isidore.MagicMirror.Users.Services;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
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

        [Fact]
        public void user_service_should_filter_correctly()
        {
            var filter = new UserFilter()
            {
                FirstName = "Kuba",
                LastName = "Matjanowski",
                RegistrationDate = new DateTime(2000, 1, 1),
                UserGuid = Guid.NewGuid().ToString("N")
            };

            _userService.GetFiltered(filter);

            A
                .CallTo(() => _collectionMock.FindAsync(
                    A<BsonDocumentFilterDefinition<User>>.Ignored,
                    A<FindOptions<User>>.Ignored,
                    A<CancellationToken>.Ignored))
                    .WhenArgumentsMatch(args =>
                    {
                        dynamic obj = JsonConvert.DeserializeObject(args.Get<BsonDocumentFilterDefinition<User>>("filter").Document.ToString());
                        Assert.Equal("Kuba", obj.FirstName.ToString());
                        Assert.Equal("Matjanowski", obj.LastName.ToString());
                        Assert.Equal(new DateTime(2000, 1, 1), (DateTime)obj.RegistrationDate);
                        Assert.Equal(filter.UserGuid, obj.UserGuid.ToString());

                        return true;
                    })
                .MustHaveHappened();

        }

        [Fact]
        public void when_filter_has_null_value_we_shoulnt_add_to_filter()
        {
            var filter = new UserFilter()
            {
                FirstName = "Kuba",
            };

            _userService.GetFiltered(filter);

            A
                .CallTo(() => _collectionMock.FindAsync(
                    A<BsonDocumentFilterDefinition<User>>.Ignored,
                    A<FindOptions<User>>.Ignored,
                    A<CancellationToken>.Ignored))
                    .WhenArgumentsMatch(args =>
                    {
                        dynamic obj = JsonConvert.DeserializeObject(args.Get<BsonDocumentFilterDefinition<User>>("filter").Document.ToString());
                        Assert.Equal("Kuba", obj.FirstName.ToString());
                        Assert.Null(obj.LastName);
                        Assert.Null(obj.RegistrationDate);
                        Assert.Null(obj.UserNo);

                        return true;

                    })
                .MustHaveHappened();

        }
    }
}
