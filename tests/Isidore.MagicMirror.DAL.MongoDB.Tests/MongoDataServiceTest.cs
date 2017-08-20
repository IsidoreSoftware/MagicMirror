using FakeItEasy;
using Isidore.MagicMirror.DAL.MongoDB;
using Isidore.MagicMirror.Infrastructure.Paging;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Isidore.MagicMirror.Infrastructure.Services;

namespace Isidore.MagicMirror.DAL.MongoDb.Tests
{
    public class MongoDataServiceTest
    {
        IMongoClient _client;
        PersonService _testService;
        private IMongoCollection<Person> _collectionMock;
        private const string DbName = "test";
        private const string CollectionName = "people-1qw2ased354rftg7uyhji8k";

        public MongoDataServiceTest()
        {
            _collectionMock = A.Fake<IMongoCollection<Person>>();
            var dbMock = A.Fake<IMongoDatabase>();
            A
                .CallTo(() => dbMock.GetCollection<Person>(A<string>.Ignored, A<MongoCollectionSettings>.Ignored))
                .Returns(_collectionMock);

            _testService = new PersonService(dbMock);
        }

        [Fact(Skip = "This test works only with MongoDB server enabled")]
        public async Task service_creates_collection()
        {
            _client = new MongoClient();
            Assert.True(await CollectionExistsAsync(_client.GetDatabase(DbName), CollectionName));

            _client.GetDatabase(DbName).DropCollection(CollectionName);
        }

        [Fact]
        public void get_all_users_return_all()
        {
            _collectionMock.ReturnsCollection(new List<Person> { new Person(), new Person(), new Person() });

            var users = _testService.GetAll().ToList();

            Assert.Equal(3, users.Count);
        }

        [Fact]
        public void get_all_users_return_all_in_pages()
        {
            var expectedPageContent = new List<Person> {
                    new Person()
                };

            var pr = new PageReqest
            {
                PageNumber = 1,
                PageSize = 1
            };
            MockExpectedResultPage(expectedPageContent, pr.PageSize, 0, 5);

            var PersonsPage = _testService.GetAll(pr);

            Assert.Equal(5, PersonsPage.TotalElementCount);
            Assert.Equal(1, PersonsPage.PageNumber);
            Assert.Equal(1, PersonsPage.PageSize);
        }

        [Fact]
        public void get_last_plus_one_page_returns_correct_page_number()
        {
            var expectedPageContent = new List<Person> {
                    new Person(),
                    new Person(),
                    new Person(),
                    new Person(),
                    new Person(),
                };

            var pr = new PageReqest();
            pr.PageNumber = 6;
            pr.PageSize = 1;
            MockExpectedResultPage(expectedPageContent, pr.PageSize, 5, 5);

            var usersPage = _testService.GetAll(pr);

            Assert.Equal(5, usersPage.PageNumber);
        }

        [Fact]
        public void get_last_plus_one_page_returns_correct_requested_page_size()
        {
            var expectedPageContent = new List<Person> {
                    new Person(),
                    new Person(),
                };

            var pr = new PageReqest();
            pr.PageNumber = 1;
            pr.PageSize = 3;
            MockExpectedResultPage(expectedPageContent, pr.PageSize, 0, 2);

            var usersPage = _testService.GetAll(pr);

            Assert.Equal(3, usersPage.RequestedPageSize);
        }

        [Fact]
        public void get_last_plus_one_page_returns_correct_actual_page_size()
        {
            var expectedPageContent = new List<Person> {
                    new Person(),
                    new Person(),
                };

            var pr = new PageReqest();
            pr.PageNumber = 1;
            pr.PageSize = 3;
            MockExpectedResultPage(expectedPageContent, pr.PageSize, 0, 2);

            var usersPage = _testService.GetAll(pr);

            Assert.Equal(2, usersPage.PageSize);
        }

        [Fact]
        public void when_requested_0_page_should_return_first()
        {
            var expectedPageContent = new List<Person> {
                    new Person(),
                    new Person(),
                };

            var pr = new PageReqest();
            pr.PageNumber = 0;
            pr.PageSize = 3;
            MockExpectedResultPage(expectedPageContent, pr.PageSize, 0, 2);

            var usersPage = _testService.GetAll(pr);

            Assert.Equal(1, usersPage.PageNumber);
        }

        [Fact]
        public void when_requested_negative_page_should_return_first()
        {
            var expectedPageContent = new List<Person> {
                    new Person(),
                    new Person(),
                };

            var pr = new PageReqest();
            pr.PageNumber = -2;
            pr.PageSize = 3;
            MockExpectedResultPage(expectedPageContent, pr.PageSize, 0, 2);

            var usersPage = _testService.GetAll(pr);

            Assert.Equal(1, usersPage.PageNumber);
        }

        [Fact(DisplayName = "Get element by Id", Skip = "Problem with Id serialization")]
        public void return_single_should_return_correct()
        {
            var expectedPageContent = new List<Person> {
                    new Person(){Name="Testov"}
                };
            var cursor = MongoTestsHelpers.GetCursor(expectedPageContent);

            A
                .CallTo(() => _collectionMock.FindAsync(
                    A<BsonDocumentFilterDefinition<Person>>
                        .That.Matches(x => x.Document == new BsonDocument("_id", new ObjectId("599824f419814e005b2308c8"))),
                    A<FindOptions<Person>>.Ignored,
                    A<CancellationToken>.Ignored))
                .Returns(cursor);

            var user = _testService.GetById("599824f419814e005b2308c8");

            Assert.NotNull(user);
        }

        [Fact]
        public void return_correct_data_on_filter()
        {
            var expectedPageContent = new List<Person> {
                    new Person(){Name="Testov"}
                };
            var cursor = MongoTestsHelpers.GetCursor(expectedPageContent);

            A
                .CallTo(() => _collectionMock.FindAsync(
                    A<BsonDocumentFilterDefinition<Person>>
                        .That.Matches(x => x.Document == new BsonDocument("Name", "Testov")),
                    A<FindOptions<Person>>.Ignored,
                    A<CancellationToken>.Ignored))
                .Returns(cursor);

            var filter = new PersonFilter() { QueryString = "{ \"Name\":\"Testov\" }"};
            var users = _testService.GetFiltered(filter);

            Assert.NotEmpty(users);
        }

        private async Task<bool> CollectionExistsAsync(IMongoDatabase database, string collectionName)
        {
            var filter = new BsonDocument("name", collectionName);
            //filter by collection name
            var collections = await database.ListCollectionsAsync(new ListCollectionsOptions { Filter = filter });
            //check for existence
            return await collections.AnyAsync();
        }

        private void MockExpectedResultPage(List<Person> expectedPageContent, int pageSize, int skip, int total)
        {
            var cursor = MongoTestsHelpers.GetCursor(expectedPageContent);

            A
                .CallTo(() => _collectionMock.FindAsync(
                    A<FilterDefinition<Person>>.Ignored,
                    A<FindOptions<Person, Person>>.That.Matches(x => x.Limit == pageSize && x.Skip == skip),
                    A<CancellationToken>.Ignored))
                .Returns(cursor);
            A
                .CallTo(() => _collectionMock.Count(
                    A<FilterDefinition<Person>>.Ignored,
                    A<CountOptions>.Ignored,
                    A<CancellationToken>.Ignored))
                .Returns(total);
        }

        private class PersonService : MongoDataService<Person>
        {
            public PersonService(IMongoDatabase database) : base(database, CollectionName)
            {
            }

            protected override string EntityIdPropertyName => "Id";
        }

        private class PersonFilter : IFilter<Person>
        {
            public string QueryString { get; set; }
        }

        public class Person : BaseMongoObject
        {
            public string Name { get; set; }
            public string Surname { get; set; }
            public int Age { get; set; }
        }
    }
}
