using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Isidore.MagicMirror.Infrastructure.Paging;
using Isidore.MagicMirror.Infrastructure.Services;
using Isidore.MagicMirror.Users.Contract;
using Isidore.MagicMirror.Users.Exceptions;
using Isidore.MagicMirror.Users.Models;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using NLog;

namespace Isidore.MagicMirror.Users.Services
{
    public class AzureUserService : IUserService
    {
        private readonly IFaceServiceClient _faceServiceClient;
        private Dictionary<string, string> _usersIdsMap = new Dictionary<string, string>();
        private readonly string _userGroupId;
        private static readonly Regex UserNameRegex = new Regex(
            @"(?<fname>([a-zA-Z])*( ?))?(?<lname>([a-zA-Z])*( ?))?(?<id>\(([a-z0-9]*)\)*)*");
        private readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public AzureUserService(IFaceServiceClient faceServiceClient, IUserGroupService userGroupService)
        {
            _faceServiceClient = faceServiceClient;
            try
            {
                _userGroupId = userGroupService.GetCurrentUserGroup().Result.Id;
            }
            catch (Exception e)
            {
                _userGroupId = null;
                Logger.Error(e,"Can't get current user group.");
            }

            BuildAzureAndDbIdMap().Wait();
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
            if (_userGroupId == null)
            {
                throw new NoDefaultUserGroupDefined();
            }

            var person = await _faceServiceClient.GetPersonAsync(_userGroupId, new Guid(_usersIdsMap[id]));
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
            if (_userGroupId == null)
            {
                throw new NoDefaultUserGroupDefined();
            }

            return (await _faceServiceClient.GetPersonsAsync(_userGroupId))
                .Select(ConvertPersonToUser);
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
            if(item.Id == null){
                throw new ArgumentException("User must have Id already known to create it on Azure.");
            }

            if (_userGroupId == null)
            {
                throw new NoDefaultUserGroupDefined();
            }

            // TODO: return id in all inserts
            var result = await _faceServiceClient.CreatePersonAsync(_userGroupId,
                   $"{item.FirstName} {item.LastName} ({item.Id})",item.Id);
            item.UserGuid = result.PersonId.ToString();

            _usersIdsMap.Add(item.Id, item.UserGuid);
        }

        public async Task UpdateAsync(string id, User item)
        {
            if (_userGroupId == null)
            {
                throw new NoDefaultUserGroupDefined();
            }

            // TODO: return number of updated records
            await _faceServiceClient.UpdatePersonAsync(_userGroupId, new Guid(_usersIdsMap[id]),
                $"{item.FirstName} {item.LastName} ({item.Id})",item.Id);
        }

        public async Task DeleteAsync(string id)
        {
            if (_userGroupId == null)
            {
                throw new NoDefaultUserGroupDefined();
            }

            // TODO: return number of deleted records
            await _faceServiceClient.DeletePersonAsync(_userGroupId, new Guid(_usersIdsMap[id]));

            if (_userGroupId.Contains(id))
            {
                _usersIdsMap.Remove(id);
            }
        }

        private async Task BuildAzureAndDbIdMap()
        {
            var persons = await _faceServiceClient.GetPersonsAsync(_userGroupId);
            foreach (var person in persons)
            {
                _usersIdsMap.Add(person.UserData,person.PersonId.ToString());
                //await _faceServiceClient.DeletePersonAsync(_userGroupId, person.PersonId);
            }
        }
    }
}
