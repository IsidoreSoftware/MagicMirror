using System;

namespace Isidore.MagicMirror.WebService.Exceptions
{
    public class ElementNotFoundException : Exception
    {
        public ElementNotFoundException(string id) : base($"Performed operation on not existing item. Id={id}.")
        {
        }
    }
}
