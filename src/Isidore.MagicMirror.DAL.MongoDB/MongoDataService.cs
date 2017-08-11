using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Isidore.MagicMirror.Infrastructure.Paging;
using Isidore.MagicMirror.Infrastructure.Services;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Collections.Concurrent;
using MongoDB.Bson.Serialization;

namespace Isidore.MagicMirror.DAL.MongoDB
{
    public abstract class MongoDataService<T> : IDataService<T>, IAsyncDataService<T> where T: BaseMongoObject
    {
        protected readonly IMongoDatabase _database;
        protected readonly IMongoCollection<T> _collection;
        protected readonly FilterDefinitionBuilder<T> _filterBuilder;

        protected MongoDataService(IMongoDatabase database, string collectionName)
        {
            _database = database;
            if (!CollectionExistsAsync(database, collectionName).Result)
            {
                database.CreateCollection(collectionName);
            }

            _collection = database.GetCollection<T>(collectionName);
            _filterBuilder = new FilterDefinitionBuilder<T>();
        }

        public IEnumerable<T> GetAll()
        {
            return this.GetAllAsync().Result;
        }

        public ResultPage<T> GetAll(PageReqest pageRequest)
        {
            return GetAllAsync(pageRequest).Result;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var filter = new BsonDocument();

            var request = await _collection.FindAsync(filter);
            var result = await ReadToEnd(request);

            return result.ToArray();
        }

        public async Task<ResultPage<T>> GetAllAsync(PageReqest pageRequest)
        {
            var filter = new BsonDocument();
            var count = _collection.Count(filter);
            var lastPage = PageCalculator.GetLastPage(count, pageRequest.PageSize);

            var options = new FindOptions<T>()
            {
                Limit = pageRequest.PageSize,
                Skip = PageCalculator.RowsToSkip(pageRequest, lastPage),
            };

            var result = new ConcurrentBag<T>();
            var request = await _collection.FindAsync(filter, options: options);
            var insertionTasks = new List<Task>();

            while (await request.MoveNextAsync())
            {
                var batch = request.Current;
                var insertion = Task.Factory.StartNew(() =>
                {
                    foreach (var item in batch)
                    {
                        result.Add(item);
                    }
                });
                insertionTasks.Add(insertion);
            }

            await Task.WhenAll(insertionTasks);
            var items = result.ToArray();
            var resultPage = new ResultPage<T>
            {
                Items = items,
                TotalElementCount = count,
                PageNumber = PageCalculator.GetActualPageNumber(pageRequest, lastPage),
                RequestedPageSize = pageRequest.PageSize,
            };

            return resultPage;
        }

        public T GetById(string id)
        {
            return GetByIdAsync(id).Result;
        }

        public async Task<T> GetByIdAsync(string id)
        {
            var r = await _collection.FindAsync<T>(Builders<T>.Filter.Eq("_id", new ObjectId(id)));
            return await r.SingleOrDefaultAsync();
        }

        public IEnumerable<T> GetFiltered(IFilter<T> filter)
        {
            return GetFilteredAsync(filter).Result;
        }

        public ResultPage<T> GetFiltered(IFilter<T> filter, PageReqest pageRequest)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<T>> GetFilteredAsync(IFilter<T> filter)
        {
            var mongoFilter = BsonSerializer.Deserialize<BsonDocument>(filter.QueryString);

            var request = (await _collection.FindAsync<T>(mongoFilter));

            return await ReadToEnd(request);
        }

        public Task<ResultPage<T>> GetFilteredAsync(IFilter<T> filter, PageReqest pageRequest)
        {
            throw new NotImplementedException();
        }

        public void Insert(T item)
        {
            InsertAsync(item).Wait();
        }

        public void Update(string id, T item)
        {
            UpdateAsync(id,item).Wait();
        }

        public async Task InsertAsync(T item)
        {
            await _collection.InsertOneAsync(item);
        }

        public async Task UpdateAsync(string id, T item)
        {
            await _collection.ReplaceOneAsync(Builders<T>.Filter.Eq("_id", new ObjectId(id)), item);
        }

        protected abstract string EntityIdPropertyName { get; }

        private static async Task<ConcurrentBag<T>> ReadToEnd(IAsyncCursor<T> request)
        {
            var result = new ConcurrentBag<T>();
            var insertionTasks = new List<Task>();
            while (await request.MoveNextAsync())
            {
                var batch = request.Current;
                var insertion = Task.Factory.StartNew(() =>
                {
                    foreach (var item in batch)
                    {
                        result.Add(item);
                    }
                });
                insertionTasks.Add(insertion);
            }

            await Task.WhenAll(insertionTasks);
            return result;
        }

        private async Task<bool> CollectionExistsAsync(IMongoDatabase database, string collectionName)
        {
            var filter = new BsonDocument("name", collectionName);
            //filter by collection name
            var collections = await database.ListCollectionsAsync(new ListCollectionsOptions { Filter = filter });
            //check for existence
            return await collections.AnyAsync();
        }
    }
}
