using Isidore.MagicMirror.Infrastructure.Paging;
using System.Collections.Generic;

namespace Isidore.MagicMirror.Infrastructure.Services
{
    public interface IDataService<T>
    {
        T GetById(string id);

        IEnumerable<T> GetFiltered(IFilter<T> filter);

        IEnumerable<T> GetAll();

        ResultPage<T> GetFiltered(IFilter<T> filter, PageReqest pageRequest);

        ResultPage<T> GetAll(PageReqest pageRequest);

        void Insert(T item);
    }
}
