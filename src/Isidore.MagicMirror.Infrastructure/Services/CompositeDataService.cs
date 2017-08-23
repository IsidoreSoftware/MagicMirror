using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Isidore.MagicMirror.Infrastructure.Paging;

namespace Isidore.MagicMirror.Infrastructure.Services
{
    public abstract class CompositeDataService<T> : ISyncAndAsyncDataService<T>
    {
        protected ISet<ISyncAndAsyncDataService<T>> Services { get; }

        protected CompositeDataService()
        {
            Services = new HashSet<ISyncAndAsyncDataService<T>>();
        }
        protected CompositeDataService(HashSet<ISyncAndAsyncDataService<T>> set)
        {
            Services = set;
        }

        public T GetById(string id)
        {
            return GetByIdAsync(id).Result;
        }

        public IEnumerable<T> GetFiltered(IFilter<T> filter)
        {
            return GetFilteredAsync(filter).Result;
        }

        public IEnumerable<T> GetAll()
        {
            return GetAllAsync().Result;
        }

        public ResultPage<T> GetFiltered(IFilter<T> filter, PageReqest pageRequest)
        {
            return GetFilteredAsync(filter, pageRequest).Result;
        }

        public ResultPage<T> GetAll(PageReqest pageRequest)
        {
            return GetAllAsync(pageRequest).Result;
        }

        public void Insert(T item)
        {
            InsertAsync(item).Wait();
        }

        public void Update(string id, T item)
        {
            UpdateAsync(id, item).Wait();
        }

        public void Delete(string id)
        {
            DeleteAsync(id).Wait();
        }

        public async Task<T> GetByIdAsync(string id)
        {
            var firstService = Services.FirstOrDefault();
            if (firstService == null)
            {
                return default(T);
            }

            return await firstService.GetByIdAsync(id);
        }

        public async Task<IEnumerable<T>> GetFilteredAsync(IFilter<T> filter)
        {
            var firstService = Services.FirstOrDefault();

            return await firstService.GetFilteredAsync(filter);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var firstService = Services.FirstOrDefault();

            return await firstService.GetAllAsync();
        }

        public async Task<ResultPage<T>> GetFilteredAsync(IFilter<T> filter, PageReqest pageRequest)
        {
            var firstService = Services.FirstOrDefault();

            return await firstService.GetFilteredAsync(filter, pageRequest);
        }

        public async Task<ResultPage<T>> GetAllAsync(PageReqest pageRequest)
        {
            var firstService = Services.FirstOrDefault();

            return await firstService.GetAllAsync(pageRequest);
        }

        public async Task InsertAsync(T item)
        {
            var firstService = Services.FirstOrDefault();

            await firstService.InsertAsync(item);
        }

        public async Task UpdateAsync(string id, T item)
        {
            var firstService = Services.FirstOrDefault();

            await firstService.UpdateAsync(id, item);
        }

        public async Task DeleteAsync(string id)
        {
            var firstService = Services.FirstOrDefault();

            await firstService.DeleteAsync(id);
        }
    }
}
