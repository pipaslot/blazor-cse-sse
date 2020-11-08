using System;
using FluentValidation;

namespace App.Server.Services
{
    public class ValidatorFactory : IValidatorFactory
    {
        private readonly IServiceProvider _provider;

        public ValidatorFactory(IServiceProvider provider)
        {
            _provider = provider;
        }

        public IValidator<T>? GetValidator<T>()
        {
            return GetValidator(typeof(T)) as IValidator<T>;
        }

        public IValidator? GetValidator(Type type)
        {
            if (type == null || type.IsValueType)
            {
                return null;
            }
            var validatorType = typeof(IValidator<>).MakeGenericType(type);
            return _provider.GetService(validatorType) as IValidator;
        }
    }
}
