using System;
using System.Collections.Generic;
using System.Text;

namespace Isidore.MagicMirror.Infrastructure.Validation
{
    internal class ValidatorComposite : IValidator
    {
        private readonly IEnumerable<IValidator> _validators;

        public ValidatorComposite(IEnumerable<IValidator> validators)
        {
            _validators = validators;
        }

        public ValidationResult Validate(object entity)
        {
            var result = new ValidationResult();
            var stringBuilder = new StringBuilder();
            bool validated = true;

            foreach (var validator in _validators)
            {
                var oneResult = validator.Validate(entity);
                validated &= oneResult.Result;
                stringBuilder.AppendLine(oneResult.Message);
            }
            var message = stringBuilder.ToString();
            result.Message = String.IsNullOrEmpty(message) ? null : message;
            result.Result = validated;
            result.Entity = entity;

            return result;
        }
    }
}
