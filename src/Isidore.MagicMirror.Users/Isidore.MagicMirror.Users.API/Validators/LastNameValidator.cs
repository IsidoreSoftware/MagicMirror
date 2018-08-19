using System;
using Isidore.MagicMirror.Infrastructure.Validation;
using Isidore.MagicMirror.Users.Models;

namespace Isidore.MagicMirror.Users.API.Validators
{
    public class LastNameValidator : AbstractValidator<User>
    {
        protected override bool GetValidationResult(User entity)
        {
            return !String.IsNullOrWhiteSpace(entity.LastName);
        }

        protected override string GetErrorMessage(User entity)
        {
            return "Last name of the user can't be empty";
        }
    }
}
