using System;

namespace Isidore.MagicMirror.Infrastructure.Exceptions
{
    public class DependentComponentException : Exception
    {
        public DependentComponentException(ComponentType type, Exception e, string additionalDetails = null)
            : base($"There is problem with dependent component: {type.ToString()}. {additionalDetails}", e)
        {
        }
    }

    public enum ComponentType
    {
        MongoDb = 1,
        InternalApi = 2,
        ExternalApi = 3,
    }
}