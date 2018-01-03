using System;
using System.Threading.Tasks;

namespace ConsulTest.Library.ServiceRegistry
{
    public interface IServiceRegistry
    {
        Task RegisterServiceAsync(IServiceRegistration serviceRegistration);
        Task DeregisterServiceAsync(IServiceRegistration serviceRegistration);
        Task<Uri> Discover(string serviceName);
    }
}
