using System;

namespace ConsulTest.Library.ServiceRegistry.Exceptions
{
    public class RegistryNotAccessibleException : Exception
    {
        public RegistryNotAccessibleException()
            : base("Service Registry not accessible.")
        {
        }
    }
}
