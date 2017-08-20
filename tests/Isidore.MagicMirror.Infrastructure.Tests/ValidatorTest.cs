using Isidore.MagicMirror.Infrastructure.Validation;
using Xunit;
using Moq;

namespace Isidore.MagicMirror.Infrastructure.Tests
{
    public class ValidatorTest
    {
        [Fact]
        public void NoValidatorValidatesObject()
        {
            var factory = new ValidatorsFactory();
            var validator = factory.GetValidator<int>();

            var result = validator.Validate(0);

            Assert.True(result.Result);
        }

        [Fact]
        public void OneValidatorCanValidateCorrectObject()
        {
            var factory = new ValidatorsFactory();
            factory.RegisterValidator<int>(new MoreThan5Validator());
            var validator = factory.GetValidator<int>();

            var result = validator.Validate(6);

            Assert.True(result.Result);
        }

        [Fact]
        public void OneValidatorCanValidateIncorrectObject()
        {
            var factory = new ValidatorsFactory();
            factory.RegisterValidator<int>(new MoreThan5Validator());
            var validator = factory.GetValidator<int>();

            var result = validator.Validate(4);

            Assert.False(result.Result);
        }

        [Fact]
        public void ValidatorDontHaveMessageOnSuccess()
        {
            var factory = new ValidatorsFactory();
            factory.RegisterValidator<int>(new MoreThan5Validator());
            var validator = factory.GetValidator<int>();

            var result = validator.Validate(6);

            Assert.Null(result.Message);
        }

        [Fact]
        public void validators_should_concat_messages_on_failure()
        {
            var factory = new ValidatorsFactory();
            factory.RegisterValidator<int>(new LessThan10Validator());
            factory.RegisterValidator<int>(new LessThan13Validator());
            var validator = factory.GetValidator<int>();

            var result = validator.Validate(14);

            Assert.False(result.Result);
            Assert.Contains("This value should be less than 10 but was 14", result.Message);
            Assert.Contains("This value should be less than 13 but was 14", result.Message);
        }

        [Fact]
        public void if_any_validation_failes_the_result_should_be_falsy()
        {
            var factory = new ValidatorsFactory();
            factory.RegisterValidator<int>(new MoreThan5Validator());
            factory.RegisterValidator<int>(new LessThan13Validator());
            var validator = factory.GetValidator<int>();

            var result = validator.Validate(2);

            Assert.False(result.Result);
        }

        [Fact]
        public void if_all_validations_passes_the_result_should_be_positive()
        {
            var factory = new ValidatorsFactory();
            factory.RegisterValidator<int>(new MoreThan5Validator());
            factory.RegisterValidator<int>(new LessThan13Validator());
            var validator = factory.GetValidator<int>();

            var result = validator.Validate(9);

            Assert.True(result.Result);
        }

        [Fact]
        public void if_all_validations_passes_the_result_message_should_be_empty()
        {
            var factory = new ValidatorsFactory();
            factory.RegisterValidator<int>(new MoreThan5Validator());
            factory.RegisterValidator<int>(new LessThan13Validator());
            var validator = factory.GetValidator<int>();

            var result = validator.Validate(9);

            Assert.Null(result.Message);
        }

        [Fact]
        public void ValidatorHaveMessageOnFailure()
        {
            var factory = new ValidatorsFactory();
            factory.RegisterValidator<int>(new MoreThan5Validator());
            var validator = factory.GetValidator<int>();

            var result = validator.Validate(4);

            Assert.Equal("This value should be more than 5 but was 4", result.Message);
        }

        [Fact]
        public void adding_the_same_validator_for_the_same_type_should_not_imply_multiple_validation()
        {
            var factory = new ValidatorsFactory();
            var validator = new Mock<IValidator>();
            validator
                .Setup(x => x.Validate(It.IsAny<int>()))
                .Returns(new ValidationResult());

            factory.RegisterValidator<int>(validator.Object);
            factory.RegisterValidator<int>(validator.Object);
            var chosenValidator = factory.GetValidator<int>();
            chosenValidator.Validate(4);

            validator.Verify(x=>x.Validate(It.IsAny<int>()), Times.Once);
        }

        #region testClasses

        private class MoreThan5Validator : AbstractValidator<int>
        {
            protected override bool GetValidationResult(int entity)
            {
                return (int)entity > 5;
            }

            protected override string GetErrorMessage(int entity)
            {
                return $"This value should be more than 5 but was {entity}";
            }
        }
        private class LessThan10Validator : AbstractValidator<int>
        {
            protected override bool GetValidationResult(int entity)
            {
                return (int)entity < 10;
            }

            protected override string GetErrorMessage(int entity)
            {
                return $"This value should be less than 10 but was {entity}";
            }
        }
        private class LessThan13Validator : AbstractValidator<int>
        {
            protected override bool GetValidationResult(int entity)
            {
                return (int)entity < 13;
            }

            protected override string GetErrorMessage(int entity)
            {
                return $"This value should be less than 13 but was {entity}";
            }
        }

        #endregion

    }
}
