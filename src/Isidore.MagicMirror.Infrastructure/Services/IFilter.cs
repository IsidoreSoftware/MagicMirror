namespace Isidore.MagicMirror.Infrastructure.Services
{
    public interface IFilter<T>
    {
        string SqlQuery { get; set; }

        // TODO: visitor implementation the same as in IQueryable
    }
}
