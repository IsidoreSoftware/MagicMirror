namespace Isidore.MagicMirror.Infrastructure.Services
{
    public interface ISyncAndAsyncDataService<T> : IDataService<T>, IAsyncDataService<T>
    {
    }
}
