using System;

namespace Isidore.MagicMirror.Infrastructure.Validation
{
    public abstract class AbstractValidator<T> : IValidator
    {
        public ValidationResult Validate(object entity)
        {
            if (typeof(T) != entity.GetType())
            {
                throw new ArgumentException($"Given value is not of the type that was defined ({entity.GetType()}!={typeof(T)})");
            }

            var validated = GetValidationResult((T)entity);

            var result = new ValidationResult
            {
                Result = validated,
                Message = validated ? null : GetErrorMessage((T)entity),
                Entity = entity
            };

            return result;
        }

        protected abstract bool GetValidationResult(T entity);

        protected abstract string GetErrorMessage(T entity);

       /* public override int GetHashCode()
        {
            // I want to have one validator for each type in a set.
            return typeof(this).GetHashCode();
        }*/
    }
}
