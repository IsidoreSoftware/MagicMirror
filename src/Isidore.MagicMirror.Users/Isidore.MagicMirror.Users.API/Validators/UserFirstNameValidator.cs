using System;
using Isidore.MagicMirror.Infrastructure.Validation;
using Isidore.MagicMirror.Users.Models;

namespace Isidore.MagicMirror.Users.API.Validators
{
    public class UserFirstNameValidator : AbstractValidator<User>
    {
        protected override bool GetValidationResult(User entity)
        {
            return !String.IsNullOrWhiteSpace(entity.FirstName);
        }

        protected override string GetErrorMessage(User entity)
        {
            return "First name of the user can't be empty";
        }
    }
}
