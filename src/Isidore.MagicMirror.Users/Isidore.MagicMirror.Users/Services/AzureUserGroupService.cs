using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Isidore.MagicMirror.Infrastructure.Paging;
using Isidore.MagicMirror.Infrastructure.Services;
using Isidore.MagicMirror.Users.Contract;
using Isidore.MagicMirror.Users.Models;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;

namespace Isidore.MagicMirror.Users.Services
{
    public class AzureUserGroupService : IUserGroupService
    {
        private readonly IFaceServiceClient _faceServiceClient;

        public AzureUserGroupService(IFaceServiceClient faceServiceClient)
        {
            _faceServiceClient = faceServiceClient;
        }

        public UserGroup GetById(string id)
        {
            return GetByIdAsync(id).Result;
        }

        public IEnumerable<UserGroup> GetFiltered(IFilter<UserGroup> filter)
        {
            return GetFilteredAsync(filter).Result;
        }

        public IEnumerable<UserGroup> GetAll()
        {
            return GetAllAsync().Result;
        }

        public ResultPage<UserGroup> GetFiltered(IFilter<UserGroup> filter, PageReqest pageRequest)
        {
            return GetFilteredAsync(filter, pageRequest).Result;
        }

        public ResultPage<UserGroup> GetAll(PageReqest pageRequest)
        {
            return this.GetAllAsync(pageRequest).Result;
        }

        public void Insert(UserGroup item)
        {
            this.InsertAsync(item).Wait();
        }

        public void Update(string id, UserGroup item)
        {
            this.UpdateAsync(id, item).Wait();
        }

        public void Delete(string id)
        {
            DeleteAsync(id).Wait();
        }

        public async Task<UserGroup> GetByIdAsync(string id)
        {
            var group = await _faceServiceClient.GetPersonGroupAsync(id);

            return new UserGroup()
            {
                Id = group.PersonGroupId,
                GroupName = group.Name
            };
        }

        public Task<IEnumerable<UserGroup>> GetFilteredAsync(IFilter<UserGroup> filter)
        {
            throw new NotSupportedException();
        }

        public async Task<IEnumerable<UserGroup>> GetAllAsync()
        {
            var groups = await _faceServiceClient.ListPersonGroupsAsync();

            return groups.Select(g => new UserGroup
            {
                Id = g.PersonGroupId,
                GroupName = g.Name
            });
        }

        public Task<ResultPage<UserGroup>> GetFilteredAsync(IFilter<UserGroup> filter, PageReqest pageRequest)
        {
            throw new NotSupportedException();
        }

        public Task<ResultPage<UserGroup>> GetAllAsync(PageReqest pageRequest)
        {
            throw new NotSupportedException();
        }

        public async Task InsertAsync(UserGroup item)
        {
           await _faceServiceClient.CreatePersonGroupAsync(item.Id, item.GroupName);
        }

        public async Task UpdateAsync(string id, UserGroup item)
        {
            await _faceServiceClient.UpdatePersonGroupAsync(id, item.GroupName);
        }

        public async Task DeleteAsync(string id)
        {
            await _faceServiceClient.DeletePersonGroupAsync(id);
        }

        public async Task<UserGroup> GetCurrentUserGroup()
        {
            var group = (await _faceServiceClient.ListPersonGroupsAsync(top:1)).FirstOrDefault();

            if (group == null)
            {
                return null;
            }

            return new UserGroup
            {
                Id = group?.PersonGroupId,
                GroupName = group?.Name
            };
        }
    }
}
