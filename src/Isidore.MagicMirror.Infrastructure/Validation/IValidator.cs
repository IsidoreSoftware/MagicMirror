namespace Isidore.MagicMirror.Infrastructure.Validation
{
    public interface IValidator
    {
        ValidationResult Validate(object entity);
    }
}
