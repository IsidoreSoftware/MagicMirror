using System;
using System.Collections.Generic;

namespace Isidore.MagicMirror.Infrastructure.Validation
{
    public class ValidatorsFactory
    {
        private readonly Dictionary<Type, HashSet<IValidator>> _validatorDictionary;
        private readonly PositiveValidator _emptyValidator;

        public ValidatorsFactory()
        {
            _validatorDictionary = new Dictionary<Type, HashSet<IValidator>>();
            _emptyValidator = new PositiveValidator();
        }
        
        public void RegisterValidator<T>(IValidator validator)
        {
            HashSet<IValidator> validators;
            if (_validatorDictionary.TryGetValue(typeof(T), out validators))
            {
                validators.Add(validator);
            }
            else
            {
                _validatorDictionary
                    .Add(typeof(T), new HashSet<IValidator> {validator});
            }
        }

        public IValidator GetValidator<TV>()
        {
            return GetValidator(typeof(TV));
        }

        public IValidator GetValidator(Type type)
        {
            HashSet<IValidator> validators;
            if (!_validatorDictionary.TryGetValue(type, out validators))
            {
                return _emptyValidator;
            }

            return new ValidatorComposite(validators);
        }
    }
}
