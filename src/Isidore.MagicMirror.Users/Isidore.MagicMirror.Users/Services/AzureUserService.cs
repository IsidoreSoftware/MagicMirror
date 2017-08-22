using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Isidore.MagicMirror.Infrastructure.Paging;
using Isidore.MagicMirror.Infrastructure.Services;
using Isidore.MagicMirror.Users.Contract;
using Isidore.MagicMirror.Users.Models;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;

namespace Isidore.MagicMirror.Users.Services
{
    public class AzureUserService : IUserService
    {
        private readonly IFaceServiceClient _faceServiceClient;
        private readonly string _userGroupId;
        private static readonly Regex UserNameRegex = new Regex(
            @"(?<fname>([a-zA-Z])*( ?))?(?<lname>([a-zA-Z])*( ?))?(?<id>\(([a-z0-9]*)\)*)*");

        public AzureUserService(IFaceServiceClient faceServiceClient, IUserGroupService userGroupService)
        {
            _faceServiceClient = faceServiceClient;
            _userGroupId = userGroupService.GetCurrentUserGroup().Result.Id;
        }

        public User GetById(string id)
        {
            return GetByIdAsync(id).Result;
        }

        public IEnumerable<User> GetFiltered(IFilter<User> filter)
        {
            return GetFilteredAsync(filter).Result;
        }

        public IEnumerable<User> GetAll()
        {
            return GetAllAsync().Result;
        }

        public ResultPage<User> GetFiltered(IFilter<User> filter, PageReqest pageRequest)
        {
            return GetFilteredAsync(filter, pageRequest).Result;
        }

        public ResultPage<User> GetAll(PageReqest pageRequest)
        {
            return GetAllAsync(pageRequest).Result;
        }

        public void Insert(User item)
        {
            InsertAsync(item).Wait();
        }

        public void Update(string id, User item)
        {
            UpdateAsync(id,item).Wait();
        }

        public void Delete(string id)
        {
            DeleteAsync(id).Wait();
        }

        public async Task<User> GetByIdAsync(string id)
        {
            var person = await _faceServiceClient.GetPersonAsync(_userGroupId, new Guid(id));
            return new User
            {
                UserGuid = person.PersonId.ToString("N"),
                Id = person.Name
            };
        }

        public Task<IEnumerable<User>> GetFilteredAsync(IFilter<User> filter)
        {
            throw new NotSupportedException();
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return (await _faceServiceClient.GetPersonsAsync(_userGroupId))
                .Select(person => ConvertPersonToUser(person));
        }

        private static User ConvertPersonToUser(Person person)
        {
            var matches = UserNameRegex.Match(person.Name);
            var user = new User
            {
                UserGuid = person.PersonId.ToString("N"),
            };

            if (matches.Success)
            {
                if (!String.IsNullOrWhiteSpace(matches.Groups["fname"].Value))
                {
                    user.FirstName = matches.Groups["fname"].Value.Trim();
                }
                if (!String.IsNullOrWhiteSpace(matches.Groups["lname"].Value))
                {
                    user.LastName = matches.Groups["lname"].Value.Trim();
                }
                if (!String.IsNullOrWhiteSpace(matches.Groups["id"].Value))
                {
                    user.Id = matches.Groups["id"].Value.Trim();
                }
            }

            return user;
        }

        public Task<ResultPage<User>> GetFilteredAsync(IFilter<User> filter, PageReqest pageRequest)
        {
            throw new NotSupportedException();
        }

        public Task<ResultPage<User>> GetAllAsync(PageReqest pageRequest)
        {
            throw new NotSupportedException();
        }

        public async Task InsertAsync(User item)
        {
            // TODO: return id in all inserts
            var result = await _faceServiceClient.CreatePersonAsync(_userGroupId,
                   $"{item.FirstName} {item.LastName} ({item.Id})");
            item.UserGuid = result.PersonId.ToString("N");
        }

        public async Task UpdateAsync(string id, User item)
        {
            // TODO: return number of updated records
            await _faceServiceClient.UpdatePersonAsync(_userGroupId, new Guid(id),
                $"{item.FirstName} {item.LastName} ({item.Id})");
        }

        public async Task DeleteAsync(string id)
        {
            // TODO: return number of deleted records
            await _faceServiceClient.DeletePersonAsync(_userGroupId, new Guid(id));
        }
    }
}
