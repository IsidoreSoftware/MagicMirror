namespace Isidore.MagicMirror.Infrastructure.Services
{
    public interface IFilter<T>
    {
        string QueryString { get; set; }

        // TODO: visitor implementation the same as in IQueryable
    }
}
