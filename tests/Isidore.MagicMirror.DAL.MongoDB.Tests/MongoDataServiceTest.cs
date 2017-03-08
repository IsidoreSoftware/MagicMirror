using FakeItEasy;
using Isidore.MagicMirror.DAL.MongoDB;
using MongoDB.Driver;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xunit;

namespace Isidore.MagicMirror.DAL.MongoDb.Tests
{
    public class MongoDataServiceTest : IDisposable
    {
        IMongoClient _client;
        PersonService _testService;
        Stopwatch _stopWatch;
        private const string DbName = "test";
        private const string CollectionName = "people";
        private const int ElementsCount = 100000;

        public MongoDataServiceTest()
        {
            _client = new MongoClient();
            _testService = new PersonService(_client.GetDatabase(DbName));
            _stopWatch = new Stopwatch();
        }

        [Fact]
        public async Task can_create_service()
        {
           // await PrepareData();
            _stopWatch.Start();
            _testService.GetAll();
            _stopWatch.Stop();

            Assert.True(_stopWatch.Elapsed < TimeSpan.FromSeconds(5),
                $"Getting {ElementsCount} elements took more than 5 sec.");
        }

        private async Task PrepareData()
        {
            _client.GetDatabase(DbName).CreateCollection(CollectionName);
            var collection = _client.GetDatabase(DbName).GetCollection<Person>(CollectionName);
            for (int i = 0; i < ElementsCount; i++)
            {
                var p = new Person
                {
                    Name = $"Tom {i}",
                    Surname = $"Riddle {i}",
                    Age = i
                };

                await collection.InsertOneAsync(p);
            }

        }

        public void Dispose()
        {
         //   _client.GetDatabase(DbName).DropCollection(CollectionName);
        }
    }

    class PersonService : MongoDataService<Person>
    {
        public PersonService(IMongoDatabase database) : base(database, "people")
        {
        }
    }

    class Person
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public int Age { get; set; }
    }
}
