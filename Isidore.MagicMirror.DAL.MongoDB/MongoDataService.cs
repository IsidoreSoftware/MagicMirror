using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Isidore.MagicMirror.Infrastructure.Paging;
using Isidore.MagicMirror.Infrastructure.Services;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Collections.Concurrent;

namespace Isidore.MagicMirror.DAL.MongoDB
{
    public abstract class MongoDataService<T> : IDataService<T>, IAsyncDataService<T>
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<T> _collection;

        public MongoDataService(IMongoDatabase database, string collectionName)
        {
            this._database = database;
            this._collection = database.GetCollection<T>(collectionName);
        }

        public IEnumerable<T> GetAll()
        {
            return this.GetAllAsync().Result;
        }

        public ResultPage<T> GetAll(PageReqest pageRequest)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var filter = new BsonDocument();
            var result = new ConcurrentBag<T>();
            var request = await _collection.FindAsync(filter);
            var insertionTasks = new List<Task>();
            while (await request.MoveNextAsync())
            {
                var batch = request.Current;
                var insertion =  Task.Factory.StartNew(() =>
                 {
                     foreach (var item in batch)
                     {
                         result.Add(item);
                     }
                 });
                insertionTasks.Add(insertion);
            }

            await Task.WhenAll(insertionTasks);

            return result.ToArray();
        }

        public Task<ResultPage<T>> GetAllAsync(PageReqest pageRequest)
        {
            throw new NotImplementedException();
        }

        public T GetById(string id)
        {
            throw new NotImplementedException();
        }

        public Task<T> GetByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetFiltered(IFilter<T> filter)
        {
            throw new NotImplementedException();
        }

        public ResultPage<T> GetFiltered(IFilter<T> filter, PageReqest pageRequest)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<T>> GetFilteredAsync(IFilter<T> filter)
        {
            throw new NotImplementedException();
        }

        public Task<ResultPage<T>> GetFilteredAsync(IFilter<T> filter, PageReqest pageRequest)
        {
            throw new NotImplementedException();
        }
    }
}
