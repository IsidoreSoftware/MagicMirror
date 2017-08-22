using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Isidore.MagicMirror.Infrastructure.Paging;
using Isidore.MagicMirror.Infrastructure.Services;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Collections.Concurrent;
using Isidore.MagicMirror.WebService.Exceptions;
using MongoDB.Bson.Serialization;

namespace Isidore.MagicMirror.DAL.MongoDB
{
    public abstract class MongoDataService<T> : IDataService<T>, IAsyncDataService<T> where T: BaseMongoObject
    {
        protected readonly IMongoDatabase Database;
        protected readonly IMongoCollection<T> Collection;
        protected readonly FilterDefinitionBuilder<T> FilterBuilder;

        protected MongoDataService(IMongoDatabase database, string collectionName)
        {
            Database = database;
            if (!CollectionExistsAsync(database, collectionName).Result)
            {
                database.CreateCollection(collectionName);
            }

            Collection = database.GetCollection<T>(collectionName);
            FilterBuilder = new FilterDefinitionBuilder<T>();
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

            var request = await Collection.FindAsync(filter);
            var result = await ReadToEnd(request);

            return result.ToArray();
        }

        public async Task<ResultPage<T>> GetAllAsync(PageReqest pageRequest)
        {
            var filter = new BsonDocument();
            var count = Collection.Count(filter);
            var lastPage = PageCalculator.GetLastPage(count, pageRequest.PageSize);

            var options = new FindOptions<T>()
            {
                Limit = pageRequest.PageSize,
                Skip = PageCalculator.RowsToSkip(pageRequest, lastPage),
            };

            var result = new ConcurrentBag<T>();
            var request = await Collection.FindAsync(filter, options: options);
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
            var filter = Builders<T>.Filter.Eq("_id", new ObjectId(id));
            var r = await Collection.FindAsync<T>(filter);
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

            var request = (await Collection.FindAsync<T>(mongoFilter));

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

        public void Delete(string id)
        {
            DeleteAsync(id).Wait();
        }

        public async Task InsertAsync(T item)
        {
            await Collection.InsertOneAsync(item);
        }

        public async Task UpdateAsync(string id, T item)
        {
            await Collection.ReplaceOneAsync(Builders<T>.Filter.Eq("_id", new ObjectId(id)), item);
        }

        public async Task DeleteAsync(string id)
        {
            var result = await Collection.DeleteOneAsync(Builders<T>.Filter.Eq("_id", new ObjectId(id)));
            if (result.DeletedCount == 0)
            {
                throw new ElementNotFoundException(id);
            }
            else if (result.DeletedCount > 1)
            {
                throw new Exception($"Removed more than 1 object for the same ID: {id}");
            }
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
