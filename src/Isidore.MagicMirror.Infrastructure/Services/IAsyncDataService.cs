using Isidore.MagicMirror.Infrastructure.Paging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Isidore.MagicMirror.Infrastructure.Services
{
    public interface IAsyncDataService<T>
    {
        Task<T> GetByIdAsync(string id);

        Task<IEnumerable<T>> GetFilteredAsync(IFilter<T> filter);

        Task<IEnumerable<T>> GetAllAsync();

        Task<ResultPage<T>> GetFilteredAsync(IFilter<T> filter, PageReqest pageRequest);

        Task<ResultPage<T>> GetAllAsync(PageReqest pageRequest);

        Task InsertAsync(T item);
    }
}
