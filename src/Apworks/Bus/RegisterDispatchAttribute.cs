using System;

namespace Apworks.Bus
{
    /// <summary>
    /// Represents that the instances of the decorated interfaces
    /// can be registered in a message dispatcher.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface)]
    public class RegisterDispatchAttribute : Attribute
    {
    }
}
