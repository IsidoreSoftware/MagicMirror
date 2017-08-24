using System;
using System.Collections.Generic;
using System.Text;

namespace Isidore.MagicMirror.Infrastructure.Validation
{
    internal class PositiveValidator:IValidator
    {
        public ValidationResult Validate(object entity)
        {
            return new ValidationResult
            {
                Message = null,
                Entity = entity,
                Result = true
            };
        }
    }
}
