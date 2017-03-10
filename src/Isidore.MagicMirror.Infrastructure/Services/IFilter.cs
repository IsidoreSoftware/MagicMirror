namespace Isidore.MagicMirror.Infrastructure.Services
{
    public interface IFilter<T>
    {
        string QueryString { get; }

        // TODO: visitor implementation the same as in IQueryable
    }
}
