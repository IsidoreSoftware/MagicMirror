using Isidore.MagicMirror.Infrastructure.Paging;

namespace Isidore.MagicMirror.Infrastructure.Services
{
    public interface IDataService<T>
    {
        T GetById(string id);

        T[] GetFiltered(IFilter<T> filter);

        T[] GetAll();

        ResultPage<T> GetFiltered(IFilter<T> filter, PageReqest pageRequest);

        ResultPage<T> GetAll(PageReqest pageRequest);
    }
}
