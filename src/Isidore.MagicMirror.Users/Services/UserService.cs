using System;
using Isidore.MagicMirror.Infrastructure.Paging;
using Isidore.MagicMirror.Infrastructure.Services;
using Isidore.MagicMirror.Users.Models;
using Isidore.MagicMirror.Users.Contract;
using MongoDB.Driver;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace Isidore.MagicMirror.Users.Services
{
    public class UserService : IUserService
    {
        private IMongoCollection<User> _usersCollection;

        public UserService(IMongoDatabase mongoDatabase)
        {
            this._usersCollection = mongoDatabase.GetCollection<User>("Users");
        }

        public IEnumerable<User> GetAll()
        {
            var filter = Builders<User>.Filter.Empty;
            var query = _usersCollection.Find(filter);
            return query.ToList();
        }

        public ResultPage<User> GetAll(PageReqest pageRequest)
        {
            var filter = Builders<User>.Filter.Empty;
            var result = new ResultPage<User>();
            result.TotalElementCount = _usersCollection.Count(filter);
            result.Items = _usersCollection.Find(filter)
                .Skip((pageRequest.PageNumber - 1) * pageRequest.PageSize)
                .Limit(pageRequest.PageSize)
                .ToList();
            result.PageNumber = pageRequest.PageNumber;
            result.RequestedPageSize = pageRequest.PageNumber;

            return result;
        }

        public User GetById(string id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<User> GetFiltered(IFilter<User> filter)
        {
            throw new NotImplementedException();
        }

        public ResultPage<User> GetFiltered(IFilter<User> filter, PageReqest pageRequest)
        {
            throw new NotImplementedException();
        }
    }
}
